using System.Security.Claims;
using eBooks.Database;
using eBooks.Models.Exceptions;
using eBooks.Models.Search;
using MapsterMapper;
using Microsoft.AspNetCore.Http;

namespace eBooks.Services
{
    public abstract class BaseCRUDService<TEntity, TSearch, TCreate, TUpdate, TResponse> : BaseReadOnlyService<TEntity, TSearch, TResponse>, IBaseReadOnlyService<TSearch, TResponse>
        where TEntity : class
        where TResponse : class
        where TSearch : BaseSearch
    {
        protected IHttpContextAccessor _httpContextAccessor;

        public BaseCRUDService(EBooksContext db, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            : base(db, mapper)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected int GetUserId() => int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : 0;

        public virtual async Task<TResponse> Post(TCreate req)
        {
            var entity = _mapper.Map<TEntity>(req);
            BeforeSaveChanges(entity);
            _db.Add(entity);
            await _db.SaveChangesAsync();
            return _mapper.Map<TResponse>(entity);
        }

        public virtual async Task<TResponse> Put(int id, TUpdate req)
        {
            var entity = await _db.Set<TEntity>().FindAsync(id);
            if (entity == null)
                throw new ExceptionNotFound();
            _mapper.Map(req, entity);
            BeforeSaveChanges(entity);
            await _db.SaveChangesAsync();
            return _mapper.Map<TResponse>(entity);
        }

        public virtual async Task<TResponse> Delete(int id)
        {
            var set = _db.Set<TEntity>();
            var entity = await set.FindAsync(id);
            if (entity == null)
                throw new ExceptionNotFound();
            set.Remove(entity);
            await _db.SaveChangesAsync();
            return null;
        }

        public virtual void BeforeSaveChanges(TEntity entity)
        {
        }
    }
}
