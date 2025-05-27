using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Models.Exceptions;
using eBooks.Models.Requests;
using eBooks.Models.Responses;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;

namespace eBooks.Services.BooksStateMachine
{
    public class DraftBooksState : BaseBooksState
    {
        protected AccountService _stripeAccountService;

        public DraftBooksState(EBooksContext db, IMapper mapper, IServiceProvider serviceProvider, ILogger<BooksService> logger, IConfiguration config)
            : base(db, mapper, serviceProvider, logger)
        {
            StripeConfiguration.ApiKey = config["Stripe:SecretKey"];
            _stripeAccountService = new AccountService();
        }

        public override async Task<BooksRes> Update(int id, BooksPutReq req)
        {
            if (req.Price < 0)
                throw new ExceptionBadRequest("Price must be zero or greater");
            if (req.LanguageId != null && !await _db.Set<Language>().AnyAsync(x => x.LanguageId == req.LanguageId))
                throw new ExceptionNotFound();
            var entity = await _db.Set<Book>().FindAsync(id);
            if (entity == null)
                throw new ExceptionNotFound();
            _mapper.Map(req, entity);
            entity.StateMachine = "draft";
            var filePath = entity.FilePath;
            if (req.BookPdfFile != null)
                await Helpers.UploadPdfFile(filePath, req.BookPdfFile, true);
            if (req.PreviewPdfFile != null)
                await Helpers.UploadPdfFile(filePath, req.PreviewPdfFile, false);
            if (req.ImageFile != null)
                await Helpers.UploadImageFile(filePath, req.ImageFile);
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
            if (!Directory.EnumerateFiles(Path.Combine(rootPath, "images"), baseFileName + ".*").Any())
                errors.AddError("Image", "You must upload book image");
            if (!Directory.EnumerateFiles(Path.Combine(rootPath, "pdfs", "books"), baseFileName + ".*").Any())
                errors.AddError("Pdf", "You must upload book pdf");
            if (!Directory.EnumerateFiles(Path.Combine(rootPath, "pdfs", "previews"), baseFileName + ".*").Any())
                errors.AddError("Pdf", "You must upload preview pdf");
            if (errors.Count > 0)
                throw new ExceptionBadRequest(errors);
            if (entity.Price > 0)
            {
                var accountService = new Stripe.AccountService();
                var account = await accountService.GetAsync(entity.Publisher.StripeAccountId);
                if (!account.DetailsSubmitted)
                    throw new ExceptionBadRequest("Stripe account is not fully onboarded");
                var transfersCapability = account.Capabilities?.Transfers;
                if (transfersCapability == null || transfersCapability != "active")
                    throw new ExceptionBadRequest("Stripe account does not have transfers capability enabled");
            }
            entity.StateMachine = "await";
            await _db.SaveChangesAsync();
            return _mapper.Map<BooksRes>(entity);
        }

        public override async Task<List<string>> AllowedActions(Book entity)
        {
            return new List<string>() { nameof(Update), nameof(Await) };
        }
    }
}
