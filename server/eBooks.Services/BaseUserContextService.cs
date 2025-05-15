using eBooks.Database;
using System.Security.Claims;
using eBooks.Interfaces;
using eBooks.Models.Exceptions;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using eBooks.Database.Models;

namespace eBooks.Services
{
    public abstract class BaseUserContextService<TEntity, TResponse> : IBaseUserContextService<TResponse>
        where TEntity : class, IUserBookEntity, new()
        where TResponse : class
    {
        protected EBooksContext _db;
        protected IMapper _mapper;
        protected IHttpContextAccessor _httpContextAccessor;

        public BaseUserContextService(EBooksContext db, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        private int GetUserId() => int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : throw new ExceptionForbidden("User not logged in");

        public virtual async Task<List<TResponse>> GetAll()
        {
            var userId = GetUserId();
            var entities = await _db.Set<TEntity>().Where(x => x.UserId == userId).ToListAsync();
            return _mapper.Map<List<TResponse>>(entities);
        }

        public virtual async Task<TResponse> GetByBookId(int bookId)
        {
            var userId = GetUserId();
            var entity = await _db.Set<TEntity>().Where(x => x.UserId == userId && x.BookId == bookId).FirstOrDefaultAsync();
            if (entity == null)
                throw new ExceptionNotFound();
            return _mapper.Map<TResponse>(entity);
        }

        public virtual async Task<TResponse> Post(int bookId)
        {
            var userId = GetUserId();
            var entity = new TEntity
            {
                UserId = userId,
                BookId = bookId
            };
            _db.Set<TEntity>().Add(entity);
            await _db.SaveChangesAsync();
            return _mapper.Map<TResponse>(entity);
        }

        public virtual async Task<TResponse> Patch(int bookId)
        {
            var userId = GetUserId();
            var entity = await _db.Set<TEntity>().FirstOrDefaultAsync(x => x.UserId == userId && x.BookId == bookId);
            if (entity == null)
                throw new ExceptionNotFound();
            entity.ModifiedAt = DateTime.Now;
            await _db.SaveChangesAsync();
            return _mapper.Map<TResponse>(entity);
        }

        public virtual async Task<TResponse> Delete(int bookId)
        {
            var set = _db.Set<TEntity>();
            var userId = GetUserId();
            var entity = await set.FirstOrDefaultAsync(x => x.UserId == userId && x.BookId == bookId);
            if (entity == null)
                throw new ExceptionNotFound();
            set.Remove(entity);
            await _db.SaveChangesAsync();
            return null;
        }
    }
}
