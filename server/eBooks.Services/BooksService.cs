using System.Security.Claims;
using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Interfaces;
using eBooks.Models;
using eBooks.Models.Books;
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
        protected BaseBooksState _baseBooksState { get; set; }
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BooksService(EBooksContext db, IMapper mapper, ILogger<BooksService> logger, BaseBooksState baseProizvodiState, IHttpContextAccessor httpContextAccessor) : base(db, mapper)
        {
            _logger = logger;
            _baseBooksState = baseProizvodiState;
            _httpContextAccessor = httpContextAccessor;
        }

        public override IQueryable<Book> AddFilter(BooksSearch search, IQueryable<Book> query)
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
            return query;
        }

        public override void BeforeCreate(Book entity, BooksCreateReq req)
        {
            entity.PublisherId = int.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : throw new ExceptionResult("You are not logged in");
            _logger.LogInformation($"Book with title {req.Title} created.");
        }

        public override BooksRes Update(int id, BooksUpdateReq req)
        {
            var entity = GetById(id);
            if (entity == null) return null;
            var state = _baseBooksState.CheckState(entity.StateMachine);
            _logger.LogInformation($"Book with title {entity.Title} updated.");
            return state.Update(id, req);
        }

        public override BooksRes Delete(int id)
        {
            var set = _db.Set<Book>();
            var entity = set.Find(id);
            if (entity == null) return null;
            entity.IsDeleted = true;
            _db.SaveChanges();
            return null;
        }

        public BooksRes UploadPdfFile(int id, Stream file)
        {
            var set = _db.Set<Book>();
            var entity = set.Find(id);
            if (entity == null) return null;
            var folderPath = Path.Combine("wwwroot", "pdfs");
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
            if (!string.IsNullOrEmpty(entity.PdfPath))
            {
                var oldFilePath = Path.Combine("wwwroot", entity.PdfPath.TrimStart('/'));
                if (File.Exists(oldFilePath)) File.Delete(oldFilePath);
            }
            var fileName = $"{Guid.NewGuid()}.pdf";
            var filePath = Path.Combine(folderPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            entity.PdfPath = $"/pdfs/{fileName}";
            _db.SaveChanges();
            return _mapper.Map<BooksRes>(entity);
        }

        public BooksRes Await(int id)
        {
            var entity = GetById(id);
            if (entity == null) throw new Exception("Book not found");
            var state = _baseBooksState.CheckState(entity.StateMachine);
            _logger.LogInformation($"Book with title {entity.Title} awaited.");
            return state.Await(id);
        }

        public BooksRes Approve(int id)
        {
            var entity = GetById(id);
            if (entity == null) throw new Exception("Book not found");
            var state = _baseBooksState.CheckState(entity.StateMachine);
            _logger.LogInformation($"Book with title {entity.Title} approved.");
            return state.Approve(id);
        }

        public BooksRes Reject(int id, string message)
        {
            var entity = GetById(id);
            if (entity == null) throw new Exception("Book not found");
            var state = _baseBooksState.CheckState(entity.StateMachine);
            _logger.LogInformation($"Book with title {entity.Title} rejected with message \"{message}\".");
            return state.Reject(id, message);
        }

        public BooksRes Hide(int id)
        {
            var entity = GetById(id);
            if (entity == null) throw new Exception("Book not found");
            var state = _baseBooksState.CheckState(entity.StateMachine);
            _logger.LogInformation($"Book with title {entity.Title} is hidden/not hidden.");
            return state.Hide(id);
        }

        public List<string> AllowedActions(int id)
        {
            var entity = _db.Books.Find(id);
            if (entity == null) throw new Exception("Book not found");
            var state = _baseBooksState.CheckState(entity.StateMachine);
            return state.AllowedActions(entity);
        }
    }
}
