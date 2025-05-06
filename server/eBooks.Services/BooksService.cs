using System.Security.Claims;
using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Interfaces;
using eBooks.Models.Books;
using eBooks.Models.Exceptions;
using eBooks.Services.BooksStateMachine;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace eBooks.Services
{
    public class BooksService : BaseService<Book, BooksSearch, BooksCreateReq, BooksUpdateReq, BooksRes>, IBooksService
    {
        protected ILogger<BooksService> _logger;
        protected BaseBooksState _baseBooksState;
        protected IHttpContextAccessor _httpContextAccessor;

        public BooksService(EBooksContext db, IMapper mapper, ILogger<BooksService> logger, BaseBooksState baseProizvodiState, IHttpContextAccessor httpContextAccessor)
            : base(db, mapper)
        {
            _logger = logger;
            _baseBooksState = baseProizvodiState;
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task<IQueryable<Book>> AddFilter(BooksSearch search, IQueryable<Book> query)
        {
            if (!string.IsNullOrWhiteSpace(search?.TitleGTE))
            {
                query = query.Where(x => x.Title.StartsWith(search.TitleGTE));
            }
            if (search?.MinPrice != null)
            {
                query = query.Where(x => x.Price >= search.MinPrice);
            }
            if (search?.MaxPrice != null)
            {
                query = query.Where(x => x.Price <= search.MinPrice);
            }
            if (!string.IsNullOrWhiteSpace(search?.StateMachine))
            {
                query = query.Where(x => x.StateMachine == "approved");
            }
            if (!string.IsNullOrWhiteSpace(search?.AuthorNameGTE))
            {
                query = query
                    .Include(x => x.BookAuthors)
                        .ThenInclude(x => x.Author)
                    .Where(x => x.BookAuthors.Any(x =>
                        (x.Author.FirstName + ' ' + x.Author.LastName).StartsWith(search.AuthorNameGTE) ||
                        (x.Author.LastName + ' ' + x.Author.FirstName).StartsWith(search.AuthorNameGTE))
                    );
            }
            if (!string.IsNullOrWhiteSpace(search?.PublisherNameGTE))
            {
                query = query
                    .Include(x => x.Publisher)
                    .Where(x =>
                        (x.Publisher.FirstName + ' ' + x.Publisher.LastName).StartsWith(search.PublisherNameGTE) ||
                        (x.Publisher.LastName + ' ' + x.Publisher.FirstName).StartsWith(search.PublisherNameGTE)
                    );
            }
            query = query.Select(x => new Book
            {
                BookId = x.BookId,
                Title = x.Title,
                Price = x.Price,
                StateMachine = x.StateMachine,
                BookImages = new List<BookImage> { x.BookImages.OrderByDescending(x => x.ModifiedAt).FirstOrDefault() }
            });
            return query;
        }

        public override async Task<BooksRes> Create(BooksCreateReq req)
        {
            var entity = _mapper.Map<Book>(req);
            entity.PublisherId = int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : 0;
            _db.Add(entity);
            await _db.SaveChangesAsync();
            if (req.Images != null && req.Images.Any()) Helpers.UploadImages(_db, _mapper, entity.BookId, req.Images);
            if (req.PdfFile != null) Helpers.UploadPdfFile(_db, _mapper, entity, req.PdfFile);
            _logger.LogInformation($"Book with title {entity.Title} created.");
            return _mapper.Map<BooksRes>(entity);
        }

        public override async Task<BooksRes> Update(int id, BooksUpdateReq req)
        {
            var entity = await GetById(id);
            if (entity == null) return null;
            var state = await _baseBooksState.CheckState(entity.StateMachine);
            _logger.LogInformation($"Book with title {entity.Title} updated.");
            return await state.Update(id, req);
        }

        public override async Task<BooksRes> Delete(int id)
        {
            var set = _db.Set<Book>();
            var entity = await set.FindAsync(id);
            if (entity == null) return null;
            entity.IsDeleted = true;
            await _db.SaveChangesAsync();
            return null;
        }

        public async Task<BookImageRes> DeleteImage(int id, int imageId)
        {
            var set = _db.Set<Book>();
            var bookEntity = await set.FindAsync(id);
            if (bookEntity == null) throw new ExceptionNotFound("Book not found");
            var bookImage = await _db.Set<BookImage>().FirstOrDefaultAsync(img => img.ImageId == imageId && img.BookId == id);
            if (bookImage == null) throw new ExceptionNotFound("Book image not found");
            var imagePath = Path.Combine("wwwroot", bookImage.ImagePath.TrimStart('/'));
            if (File.Exists(imagePath)) File.Delete(imagePath);
            _db.Set<BookImage>().Remove(bookImage);
            await _db.SaveChangesAsync();
            return null;
        }

        public async Task<BooksRes> Await(int id)
        {
            var entity = await GetById(id);
            if (entity == null) throw new ExceptionNotFound("Book not found");
            var state = await _baseBooksState.CheckState(entity.StateMachine);
            _logger.LogInformation($"Book with title {entity.Title} awaited.");
            return await state.Await(id);
        }

        public async Task<BooksRes> Approve(int id)
        {
            var entity = await GetById(id);
            if (entity == null) throw new ExceptionNotFound("Book not found");
            var state = await _baseBooksState.CheckState(entity.StateMachine);
            _logger.LogInformation($"Book with title {entity.Title} approved.");
            return await state.Approve(id);
        }

        public async Task<BooksRes> Reject(int id, string message)
        {
            var entity = await GetById(id);
            if (entity == null) throw new ExceptionNotFound("Book not found");
            var state = await _baseBooksState.CheckState(entity.StateMachine);
            _logger.LogInformation($"Book with title {entity.Title} rejected with message \"{message}\".");
            return await state.Reject(id, message);
        }

        public async Task<BooksRes> Hide(int id)
        {
            var entity = await GetById(id);
            if (entity == null) throw new ExceptionNotFound("Book not found");
            var state = await _baseBooksState.CheckState(entity.StateMachine);
            _logger.LogInformation($"Book with title {entity.Title} is hidden/not hidden.");
            return await state.Hide(id);
        }

        public async Task<List<string>> AllowedActions(int id)
        {
            var entity = await _db.Books.FindAsync(id);
            if (entity == null) throw new ExceptionNotFound("Book not found");
            var state = await _baseBooksState.CheckState(entity.StateMachine);
            return await state.AllowedActions(entity);
        }
    }
}
