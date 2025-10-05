using System.Security.Claims;
using Azure;
using EasyNetQ;
using EasyNetQ.Internals;
using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Interfaces;
using eBooks.Models.Exceptions;
using eBooks.Models.Messages;
using eBooks.Models.Requests;
using eBooks.Models.Responses;
using eBooks.Models.Search;
using eBooks.Services.BooksStateMachine;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace eBooks.Services
{
    public class BooksService : BaseCRUDService<Book, BooksSearch, BooksPostReq, BooksPutReq, BooksRes>, IBooksService
    {
        protected BaseBooksState _baseBooksState;
        protected IBus _bus;
        protected ILogger<BooksService> _logger;

        public BooksService(EBooksContext db, IMapper mapper, IHttpContextAccessor httpContextAccessor, BaseBooksState baseBooksState, IBus bus, ILogger<BooksService> logger)
            : base(db, mapper, httpContextAccessor)
        {
            _baseBooksState = baseBooksState;
            _bus = bus;
            _logger = logger;
        }

        protected async Task<bool> AccessRightExist(int bookId) => await _db.Set<AccessRight>().AnyAsync(x => x.UserId == GetUserId() && x.BookId == bookId);

        public override IQueryable<Book> AddIncludes(IQueryable<Book> query, BooksSearch? search = null)
        {
            query = query.Include(x => x.Publisher);
            query = query.Include(x => x.Language);
            if (search == null || search.IsAuthorsIncluded == true)
                query = query.Include(x => x.BookAuthors).ThenInclude(x => x.Author);
            if (search == null || search.IsGenresIncluded == true)
                query = query.Include(x => x.BookGenres).ThenInclude(x => x.Genre);
            if (search == null || search.IsReviewsIncluded == true)
                query = query.Include(x => x.Reviews);
            return query;
        }

        public override IQueryable<Book> AddFilters(IQueryable<Book> query, BooksSearch search)
        {
            if (!string.IsNullOrWhiteSpace(search.Title))
                query = query.Where(x => x.Title.ToLower().Contains(search.Title.ToLower()));
            if (search.PublisherId != null)
                query = query.Where(x => x.PublisherId == search.PublisherId);
            if (!string.IsNullOrWhiteSpace(search.Publisher))
                query = query.Where(x => x.Publisher.UserName.ToLower().Contains(search.Publisher.ToLower()));
            if (!string.IsNullOrWhiteSpace(search.Author))
                query = query.Where(x => x.BookAuthors.Any(x => x.Author.Name.ToLower().Contains(search.Author.ToLower())));
            if (!string.IsNullOrWhiteSpace(search.Genre))
                query = query.Where(x => x.BookGenres.Any(x => x.Genre.Name.ToLower().Contains(search.Genre.ToLower())));
            if (!string.IsNullOrWhiteSpace(search.Language))
                query = query.Where(x => x.Language.Name.ToLower().Contains(search.Language.ToLower()));
            if (search.MinPrice != null)
                query = query.Where(x => (x.DiscountPercentage != null && x.DiscountStart <= DateTime.Now && x.DiscountEnd >= DateTime.Now) ? (x.Price - (x.Price * x.DiscountPercentage.Value / 100)) >= search.MinPrice : x.Price >= search.MinPrice);
            if (search.MaxPrice != null)
                query = query.Where(x => (x.DiscountPercentage != null && x.DiscountStart <= DateTime.Now && x.DiscountEnd >= DateTime.Now) ? (x.Price - (x.Price * x.DiscountPercentage.Value / 100)) <= search.MaxPrice : x.Price <= search.MaxPrice);
            query = search.Status switch
            {
                "Approved" => query.Where(x => x.StateMachine == "approve"),
                "Awaited" => query.Where(x => x.StateMachine == "await"),
                "Drafted" => query.Where(x => x.StateMachine == "draft"),
                "Hidden" => query.Where(x => x.StateMachine == "hide"),
                "Rejected" => query.Where(x => x.StateMachine == "reject"),
                _ => query,
            };
            if (search.IsDeleted == "Not deleted")
                query = query.Where(x => x.DeletionReason == null);
            else if (search.IsDeleted == "Deleted")
                query = query.Where(x => x.DeletionReason != null);
            if (search.FollowingPublishersOnly == true)
                query = query.Where(x => _db.Set<PublisherFollow>().Any(y => y.PublisherId == x.PublisherId && y.UserId == GetUserId()));
            if (search?.OrderBy == "Highest rated")
            {
                query = query.Select(x => new
                {
                    Book = x,
                    AverageRating = x.Reviews.Any() ? x.Reviews.Average(y => y.Rating) : 0
                }).OrderByDescending(x => x.AverageRating).Select(x => x.Book);
            }
            else
            {
                query = search?.OrderBy switch
                {
                    "Most views" => query.OrderByDescending(x => x.NumberOfViews),
                    "Lowest price" => query.OrderBy(x => x.Price),
                    "Highest price" => query.OrderByDescending(x => x.Price),
                    "Title" => query.OrderBy(x => x.Title),
                    "Publisher" => query.OrderBy(x => x.Publisher.UserName),
                    _ => query.OrderByDescending(x => x.ModifiedAt),
                };
            }
            return query;
        }

        public override async Task<PagedResult<BooksRes>> GetPaged(BooksSearch search)
        {
            var query = _db.Set<Book>().AsQueryable();
            query = AddIncludes(query);
            query = AddFilters(query, search);
            int count = await query.CountAsync();
            if (search?.Page.HasValue == true && search?.PageSize.HasValue == true && search.Page.Value > 0)
                query = query.Skip((search.Page.Value - 1) * search.PageSize.Value).Take(search.PageSize.Value);
            var list = await query.ToListAsync();
            var originalEntitiesById = list.ToDictionary(x => x.BookId);
            TypeAdapterConfig<Book, BooksRes>.NewConfig().Map(x => x.Status, src => MapState(src.StateMachine));
            var result = list.Adapt<List<BooksRes>>();
            result = result.Select(book =>
            {
                book.BookAuthors = book.BookAuthors.OrderByDescending(x => x.ModifiedAt).ToList();
                book.BookGenres = book.BookGenres.OrderByDescending(x => x.ModifiedAt).ToList();
                if (originalEntitiesById.TryGetValue(book.BookId, out var originalEntity) && originalEntity.Reviews?.Any() == true)
                    book.AverageRating = Math.Round(originalEntity.Reviews.Average(x => x.Rating), 1);
                else
                    book.AverageRating = 0;
                return book;
            }).ToList();
            var pagedResult = new PagedResult<BooksRes>
            {
                ResultList = result,
                Count = count
            };
            return pagedResult;
        }

        public override async Task<BooksRes> GetById(int id)
        {
            var query = _db.Set<Book>().AsQueryable();
            query = AddIncludes(query);
            var entity = query.FirstOrDefault(x => x.BookId == id);
            if (entity == null)
                throw new ExceptionNotFound();
            var result = _mapper.Map<BooksRes>(entity);
            entity.NumberOfViews += 1;
            await _db.SaveChangesAsync();
            result.BookAuthors = result.BookAuthors.OrderByDescending(x => x.ModifiedAt).ToList();
            result.BookGenres = result.BookGenres.OrderByDescending(x => x.ModifiedAt).ToList();
            result.AverageRating = (entity.Reviews != null && entity.Reviews.Any()) ? Math.Round(entity.Reviews.Average(x => x.Rating), 1) : 0;
            result.Status = MapState(entity.StateMachine);
            return result;
        }

        public override async Task<BooksRes> Post(BooksPostReq req)
        {
            var errors = new Dictionary<string, List<string>>();
            if (req.Price < 0)
                errors.AddError("Price", "Price must be zero or greater");
            if (req.NumberOfPages < 1)
                errors.AddError("Pages", "Number of pages must be greater than zero");
            var filePath = $"{Guid.NewGuid():N}";
            TypeAdapterConfig<BooksPostReq, Book>.NewConfig().Ignore(x => x.Language);
            var entity = _mapper.Map<Book>(req);
            entity.FilePath = filePath;
            entity.PublisherId = GetUserId();
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
                    ModifiedAt = now.AddSeconds(-i)
                });
            }
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
                    ModifiedAt = now.AddSeconds(-i)
                });
            }
            if (errors.Count > 0)
                throw new ExceptionBadRequest(errors);
            if (req.BookPdfFile != null)
                await Helpers.UploadPdfFile(filePath, req.BookPdfFile, true);
            if (req.SummaryPdfFile != null)
                await Helpers.UploadPdfFile(filePath, req.SummaryPdfFile, false);
            if (req.ImageFile != null)
                await Helpers.UploadImageFile(filePath, req.ImageFile, true);
            _db.Add(entity);
            await _db.SaveChangesAsync();
            _logger.LogInformation($"Book with title {entity.Title} created.");
            return _mapper.Map<BooksRes>(entity);
        }

        public override async Task<BooksRes> Put(int id, BooksPutReq req)
        {
            if (req.Price < 0)
                throw new ExceptionBadRequest("Price must be zero or greater");
            var entity = await _db.Set<Book>().FindAsync(id);
            if (entity == null)
                throw new ExceptionNotFound();
            var state = _baseBooksState.CheckState(entity.StateMachine);
            return await state.Update(id, req);
        }

        public override async Task<BooksRes> Delete(int id)
        {
            var entity = await _db.Set<Book>().Include(x => x.Publisher).FirstOrDefaultAsync(x => x.BookId == id);
            if (entity == null)
                throw new ExceptionNotFound();
            entity.RejectionReason = "Deleted by user";
            _logger.LogInformation($"Book with title {entity.Title} deleted.");
            var result = _mapper.Map<BooksRes>(entity);
            _bus.PubSub.Publish(new BookDeactivated { Book = result });
            string notificationMessage;
            if (result.Publisher.DeletionReason != null)
                notificationMessage = $"Your book has been deactivated. Reason: {result.DeletionReason}";
            else
                notificationMessage = "Your book has been reactivated";
            var userId = result.Publisher.UserId;
            var notification = new Notification
            {
                UserId = userId,
                BookId = result.BookId,
                Message = notificationMessage
            };
            _db.Set<Notification>().Add(notification);
            await _db.SaveChangesAsync();
            return result;
        }

        public async Task<BooksRes> DeleteByAdmin(int id, string? reason)
        {
            var entity = await _db.Set<Book>().Include(x => x.Publisher).FirstOrDefaultAsync(x => x.BookId == id);
            if (entity == null)
                throw new ExceptionNotFound();
            if (entity.DeletionReason == null && reason == null)
                throw new ExceptionBadRequest("Not deleted");
            if (entity.DeletionReason != null && reason != null)
                throw new ExceptionBadRequest("Already deleted");
            entity.DeletionReason = reason;
            _logger.LogInformation($"Book with title {entity.Title} deleted.");
            var result = _mapper.Map<BooksRes>(entity);
            _bus.PubSub.Publish(new BookDeactivated { Book = result });
            string notificationMessage;
            if (result.Publisher.DeletionReason != null)
                notificationMessage = $"Your book has been deactivated. Reason: {result.DeletionReason}";
            else
                notificationMessage = "Your book has been reactivated";
            var userId = result.Publisher.UserId;
            var notification = new Notification
            {
                UserId = userId,
                BookId = result.BookId,
                Message = notificationMessage
            };
            _db.Set<Notification>().Add(notification);
            await _db.SaveChangesAsync();
            return result;
        }

        public async Task<BooksRes> SetDiscount(int id, DiscountReq req)
        {
            var errors = new Dictionary<string, List<string>>();
            var entity = await _db.Set<Book>().FindAsync(id);
            if (entity == null)
                throw new ExceptionNotFound();
            if (req.DiscountPercentage <= 0 || req.DiscountPercentage >= 100)
                errors.AddError("Discount", "Discount must be between 0 and 100");
            if (req.DiscountStart >= req.DiscountEnd)
                errors.AddError("Discount", "Discount start date must be before discount end date");
            if (entity.Price == 0)
                errors.AddError("Book", "You cannot set discount to a free book");
            if (errors.Count > 0)
                throw new ExceptionBadRequest(errors);
            _mapper.Map(req, entity);
            _logger.LogInformation($"Book with title {entity.Title} is discounted by {req.DiscountPercentage}%.");
            var result = _mapper.Map<BooksRes>(entity);
            _bus.PubSub.Publish(new PublisherFollowing { Book = result, Action = "added discount to a" });
            _bus.PubSub.Publish(new BookDiscounted { Book = result });
            var userIds = await _db.Set<Wishlist>().Where(x => x.BookId == result.BookId).Select(x => x.UserId).ToListAsync();
            var notifications = userIds.Select(userId => new Notification
            {
                UserId = userId,
                BookId = result.BookId,
                Message = $"Book \"{result.Title}\" is on discount, new price is {Helpers.CalculateDiscountedPrice(result.Price, result.DiscountPercentage, result.DiscountStart, result.DiscountEnd)}"
            }).ToList();
            _db.Set<Notification>().AddRange(notifications);
            var userIds2 = await _db.Set<PublisherFollow>().Where(x => x.PublisherId == result.Publisher.UserId).Select(x => x.UserId).ToListAsync();
            var notifications2 = userIds2.Select(userId => new Notification
            {
                PublisherId = result.Publisher.UserId,
                UserId = userId,
                Message = $"Publisher {result.Publisher.UserName} set discount to a book {result.Title}"
            }).ToList();
            _db.Set<Notification>().AddRange(notifications2);
            await _db.SaveChangesAsync();
            return result;
        }

        public async Task<Tuple<string, byte[]>> GetBookFile(int id)
        {
            var entity = await _db.Set<Book>().FindAsync(id);
            if (entity == null)
                throw new ExceptionNotFound();
            var role = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
            if (!await AccessRightExist(id) && entity.PublisherId != GetUserId() && role != "Admin" && role != "Moderator")
                throw new ExceptionForbidden("You do not have access to this book");
            var fileName = $"{entity.FilePath}.pdf";
            var filePath = Path.Combine("wwwroot", "pdfs", "books", fileName);
            if (!System.IO.File.Exists(filePath))
                throw new ExceptionBadRequest($"PDF file does not exist");
            var fileContent = await System.IO.File.ReadAllBytesAsync(filePath);
            return new Tuple<string, byte[]>(fileName, fileContent);
        }

        public async Task<BooksRes> Await(int id)
        {
            var entity = await _db.Set<Book>().FindAsync(id);
            if (entity == null)
                throw new ExceptionNotFound();
            var state = _baseBooksState.CheckState(entity.StateMachine);
            return await state.Await(id);
        }

        public async Task<BooksRes> Approve(int id)
        {
            var entity = await _db.Set<Book>().FindAsync(id);
            if (entity == null)
                throw new ExceptionNotFound();
            var state = _baseBooksState.CheckState(entity.StateMachine);
            return await state.Approve(id);
        }

        public async Task<BooksRes> Reject(int id, string reason)
        {
            var entity = await _db.Set<Book>().FindAsync(id);
            if (entity == null)
                throw new ExceptionNotFound();
            var state = _baseBooksState.CheckState(entity.StateMachine);
            return await state.Reject(id, reason);
        }

        public async Task<BooksRes> Hide(int id)
        {
            var entity = await _db.Set<Book>().FindAsync(id);
            if (entity == null)
                throw new ExceptionNotFound();
            var state = _baseBooksState.CheckState(entity.StateMachine);
            return await state.Hide(id);
        }

        public async Task<List<string>> AdminAllowedActions(int id)
        {
            var entity = await _db.Set<Book>().FindAsync(id);
            if (entity == null)
                throw new ExceptionNotFound();
            var state = _baseBooksState.CheckState(entity.StateMachine);
            return state.AdminAllowedActions();
        }

        public async Task<List<string>> UserAllowedActions(int id)
        {
            var entity = await _db.Set<Book>().FindAsync(id);
            if (entity == null)
                throw new ExceptionNotFound();
            var state = _baseBooksState.CheckState(entity.StateMachine);
            return state.UserAllowedActions();
        }

        public async Task<List<string>> BookStates()
        {
            return ["Approved", "Awaited", "Drafted", "Hidden", "Rejected"];
        }

        protected string MapState(string state)
        {
            return state switch
            {
                "draft" => "Drafted",
                "await" => "Awaited",
                "approve" => "Approved",
                "reject" => "Rejected",
                "hide" => "Hidden",
                _ => state ?? string.Empty
            };
        }
    }
}
