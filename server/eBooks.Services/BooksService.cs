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

        public BooksService(EBooksContext db, IMapper mapper, ILogger<BooksService> logger, BaseBooksState baseBooksState, IHttpContextAccessor httpContextAccessor)
            : base(db, mapper)
        {
            _logger = logger;
            _baseBooksState = baseBooksState;
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task<BooksRes> Create(BooksCreateReq req)
        {
            var entity = _mapper.Map<Book>(req);
            entity.PublisherId = int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var temp) ? temp : throw new ExceptionForbidden("User not logged in");
            _db.Add(entity);
            await _db.SaveChangesAsync();
            if (req.Images != null && req.Images.Any())
                Helpers.UploadImages(_db, _mapper, entity.BookId, req.Images);
            if (req.PdfFile != null)
                Helpers.UploadPdfFile(_db, _mapper, entity, req.PdfFile);
            _logger.LogInformation($"Book with title {entity.Title} created.");
            return _mapper.Map<BooksRes>(entity);
        }

        public override async Task<BooksRes> Update(int id, BooksUpdateReq req)
        {
            var entity = await GetById(id);
            if (entity == null)
                throw new ExceptionNotFound();
            var state = await _baseBooksState.CheckState(entity.StateMachine);
            _logger.LogInformation($"Book with title {entity.Title} updated.");
            return await state.Update(id, req);
        }

        public override async Task<BooksRes> Delete(int id)
        {
            var set = _db.Set<Book>();
            var entity = await set.FindAsync(id);
            if (entity == null)
                throw new ExceptionNotFound();
            entity.IsDeleted = true;
            await _db.SaveChangesAsync();
            return null;
        }

        public async Task<BooksRes> UndoDelete(int id)
        {
            var set = _db.Set<Book>();
            var entity = await set.FindAsync(id);
            if (entity == null)
                throw new ExceptionNotFound();
            entity.IsDeleted = false;
            await _db.SaveChangesAsync();
            return _mapper.Map<BooksRes>(entity);
        }

        public async Task<BookImageRes> DeleteImage(int id, int imageId)
        {
            var bookEntity = await _db.Set<Book>().FindAsync(id);
            if (bookEntity == null)
                throw new ExceptionNotFound();
            var set = _db.Set<BookImage>();
            var bookImage = await set.FirstOrDefaultAsync(img => img.ImageId == imageId && img.BookId == id);
            if (bookImage == null)
                throw new ExceptionNotFound();
            var imagePath = Path.Combine("wwwroot", bookImage.ImagePath.TrimStart('/'));
            if (File.Exists(imagePath)) File.Delete(imagePath);
            set.Remove(bookImage);
            await _db.SaveChangesAsync();
            return null;
        }

        public async Task<BooksRes> Await(int id)
        {
            var entity = await GetById(id);
            if (entity == null)
                throw new ExceptionNotFound();
            var state = await _baseBooksState.CheckState(entity.StateMachine);
            _logger.LogInformation($"Book with title {entity.Title} awaited.");
            return await state.Await(id);
        }

        public async Task<BooksRes> Approve(int id)
        {
            var entity = await GetById(id);
            if (entity == null)
                throw new ExceptionNotFound();
            var state = await _baseBooksState.CheckState(entity.StateMachine);
            _logger.LogInformation($"Book with title {entity.Title} approved.");
            return await state.Approve(id);
        }

        public async Task<BooksRes> Reject(int id, string message)
        {
            var entity = await GetById(id);
            if (entity == null)
                throw new ExceptionNotFound();
            var state = await _baseBooksState.CheckState(entity.StateMachine);
            _logger.LogInformation($"Book with title {entity.Title} rejected with message \"{message}\".");
            return await state.Reject(id, message);
        }

        public async Task<BooksRes> Hide(int id)
        {
            var entity = await GetById(id);
            if (entity == null)
                throw new ExceptionNotFound();
            var state = await _baseBooksState.CheckState(entity.StateMachine);
            _logger.LogInformation($"Book with title {entity.Title} is hidden/not hidden.");
            return await state.Hide(id);
        }

        public async Task<List<string>> AllowedActions(int id)
        {
            var entity = await _db.Books.FindAsync(id);
            if (entity == null)
                throw new ExceptionNotFound();
            var state = await _baseBooksState.CheckState(entity.StateMachine);
            return await state.AllowedActions(entity);
        }
    }
}
