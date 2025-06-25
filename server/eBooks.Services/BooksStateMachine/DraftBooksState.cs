using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Models.Exceptions;
using eBooks.Models.Requests;
using eBooks.Models.Responses;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;

namespace eBooks.Services.BooksStateMachine
{
    public class DraftBooksState : BaseBooksState
    {
        protected AccountService _stripeAccountService;

        public DraftBooksState(EBooksContext db, IMapper mapper, IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider, ILogger<BooksService> logger, IConfiguration config)
            : base(db, mapper, httpContextAccessor, serviceProvider, logger)
        {
            StripeConfiguration.ApiKey = config["Stripe:SecretKey"];
            _stripeAccountService = new AccountService();
        }

        public override async Task<BooksRes> Update(int id, BooksPutReq req)
        {
            var errors = new Dictionary<string, List<string>>();
            if (req.Price < 0)
                errors.AddError("Price", "Price must be zero or greater");
            if (req.NumberOfPages < 1)
                errors.AddError("Pages", "Number of pages must greater then zero");
            var entity = await _db.Set<Book>().Include(x => x.BookAuthors).Include(x => x.BookGenres).FirstOrDefaultAsync(x => x.BookId == id);
            if (entity == null)
                throw new ExceptionNotFound();
            TypeAdapterConfig<BooksPutReq, Book>.NewConfig().Ignore(x => x.Language);
            _mapper.Map(req, entity);
            entity.StateMachine = "draft";
            if (!string.IsNullOrWhiteSpace(req.Language))
            {
                var language = await _db.Languages.FirstOrDefaultAsync(x => x.Name.ToLower() == req.Language.ToLower());
                if (language == null)
                {
                    language = new Language
                    {
                        Name = req.Language,
                        ModifiedAt = DateTime.UtcNow,
                        ModifiedById = GetUserId()
                    };
                    _db.Languages.Add(language);
                    await _db.SaveChangesAsync();
                }
                entity.Language = language;
            }
            var now = DateTime.UtcNow;
            var existingBookAuthors = await _db.Set<BookAuthor>().Where(x => x.BookId == entity.BookId).ToListAsync();
            _db.Set<BookAuthor>().RemoveRange(existingBookAuthors);
            List<string> authorsList = [];
            if (!string.IsNullOrWhiteSpace(req.Authors))
            {
                try
                {
                    authorsList = System.Text.Json.JsonSerializer.Deserialize<List<string>>(req.Authors) ?? [];
                }
                catch
                {
                    errors.AddError("Authors", "Invalid format");
                }
            }
            for (int i = 0; i < authorsList.Count; i++)
            {
                var authorName = authorsList[i];
                var author = await _db.Authors.FirstOrDefaultAsync(x => x.Name.ToLower() == authorName.ToLower());
                if (author == null)
                {
                    author = new Author
                    {
                        Name = authorName,
                        ModifiedAt = now.AddSeconds(-i),
                        ModifiedById = GetUserId()
                    };
                    _db.Authors.Add(author);
                    await _db.SaveChangesAsync();
                }
                entity.BookAuthors.Add(new BookAuthor
                {
                    AuthorId = author.AuthorId,
                    BookId = entity.BookId,
                    ModifiedAt = now.AddSeconds(-i)
                });
            }
            var existingBookGenres = await _db.Set<BookGenre>().Where(x => x.BookId == entity.BookId).ToListAsync();
            _db.Set<BookGenre>().RemoveRange(existingBookGenres);
            List<string> genresList = [];
            if (!string.IsNullOrWhiteSpace(req.Genres))
            {
                try
                {
                    genresList = System.Text.Json.JsonSerializer.Deserialize<List<string>>(req.Genres) ?? [];
                }
                catch
                {
                    errors.AddError("Genres", "Invalid format");
                }
            }
            for (int i = 0; i < genresList.Count; i++)
            {
                var genreName = genresList[i];
                var genre = await _db.Genres.FirstOrDefaultAsync(x => x.Name.ToLower() == genreName.ToLower());
                if (genre == null)
                {
                    genre = new Genre
                    {
                        Name = genreName,
                        ModifiedAt = now.AddSeconds(-i),
                        ModifiedById = GetUserId()
                    };
                    _db.Genres.Add(genre);
                    await _db.SaveChangesAsync();
                }
                entity.BookGenres.Add(new BookGenre
                {
                    GenreId = genre.GenreId,
                    BookId = entity.BookId,
                    ModifiedAt = now.AddSeconds(-i)
                });
            }
            if (errors.Count > 0)
                throw new ExceptionBadRequest(errors);
            var filePath = entity.FilePath;
            if (req.BookPdfFile != null)
                await Helpers.UploadPdfFile(filePath, req.BookPdfFile, true);
            if (req.SummaryPdfFile != null)
                await Helpers.UploadPdfFile(filePath, req.SummaryPdfFile, false);
            if (req.ImageFile != null)
                await Helpers.UploadImageFile(filePath, req.ImageFile, true);
            await _db.SaveChangesAsync();
            _logger.LogInformation($"Book with title {entity.Title} updated.");
            return _mapper.Map<BooksRes>(entity);
        }

        public override async Task<BooksRes> Await(int id)
        {
            var entity = await _db.Set<Book>().Include(x => x.Publisher).FirstOrDefaultAsync(x => x.BookId == id);
            if (entity == null)
                throw new ExceptionNotFound();
            var errors = new Dictionary<string, List<string>>();
            var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var baseFileName = Path.GetFileNameWithoutExtension(entity.FilePath ?? "");
            if (string.IsNullOrWhiteSpace(entity.Title) || string.IsNullOrWhiteSpace(entity.Description) || entity.Price == null || entity.NumberOfPages == null || entity.LanguageId == 0)
                errors.AddError("Data", "You must fill out all data before awaiting book");
            if (!Directory.EnumerateFiles(Path.Combine(rootPath, "images", "books"), baseFileName + ".*").Any())
                errors.AddError("Image", "You must upload book image");
            if (!Directory.EnumerateFiles(Path.Combine(rootPath, "pdfs", "books"), baseFileName + ".*").Any())
                errors.AddError("Pdf", "You must upload book pdf");
            if (!Directory.EnumerateFiles(Path.Combine(rootPath, "pdfs", "summary"), baseFileName + ".*").Any())
                errors.AddError("Pdf", "You must upload summary pdf");
            if (!await _db.Set<BookAuthor>().AnyAsync(x => x.BookId == id))
                errors.AddError("Authors", "You must select at least one author");
            if (!await _db.Set<BookGenre>().AnyAsync(x => x.BookId == id))
                errors.AddError("Genres", "You must select at least one genre");
            if (errors.Count > 0)
                throw new ExceptionBadRequest(errors);
            if (entity.Price > 0)
            {
                var accountService = new Stripe.AccountService();
                var account = await accountService.GetAsync(entity.Publisher.StripeAccountId);
                if (!account.DetailsSubmitted)
                    throw new ExceptionBadRequest("Stripe account is not fully set up");
                var transfersCapability = account.Capabilities?.Transfers;
                if (transfersCapability == null || transfersCapability != "active")
                    throw new ExceptionBadRequest("Stripe account does not have transfers capability enabled");
            }
            entity.StateMachine = "await";
            await _db.SaveChangesAsync();
            return _mapper.Map<BooksRes>(entity);
        }

        public override List<string> AdminAllowedActions()
        {
            return new List<string>() { };
        }

        public override List<string> UserAllowedActions()
        {
            return new List<string>() { nameof(Update), nameof(Await) };
        }
    }
}
