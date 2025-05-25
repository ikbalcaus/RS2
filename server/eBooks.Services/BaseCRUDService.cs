using eBooks.Database;
using eBooks.Models.Exceptions;
using eBooks.Models.SearchObjects;
using MapsterMapper;

namespace eBooks.Services
{
    public abstract class BaseCRUDService<TEntity, TSearch, TCreate, TUpdate, TResponse> : BaseReadOnlyService<TEntity, TSearch, TResponse>, IBaseReadOnlyService<TSearch, TResponse>
        where TEntity : class
        where TResponse : class
        where TSearch : BaseSearch
    {
        public BaseCRUDService(EBooksContext db, IMapper mapper)
            : base(db, mapper)
        {
        }

        public virtual async Task<TResponse> Post(TCreate req)
        {
            var entity = _mapper.Map<TEntity>(req);
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
    }
}
