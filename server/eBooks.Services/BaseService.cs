using eBooks.Database;
using eBooks.Models;
using eBooks.Models.Exceptions;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace eBooks.Services
{
    public abstract class BaseService<TEntity, TSearch, TCreate, TUpdate, TResponse> : BaseReadOnlyService<TSearch, TEntity, TResponse>
        where TResponse : class
        where TSearch : BaseSearch
        where TEntity : class
    {
        public BaseService(EBooksContext db, IMapper mapper)
            : base(db, mapper)
        {
        }

        public virtual async Task<TResponse> Create(TCreate req)
        {
            TEntity entity = _mapper.Map<TEntity>(req);
            BeforeCreate(entity, req);
            _db.Add(entity);
            await _db.SaveChangesAsync();
            return _mapper.Map<TResponse>(entity);
        }

        public virtual async Task<TResponse> Update(int id, TUpdate req)
        {
            var set = _db.Set<TEntity>();
            var entity = await set.FindAsync(id);
            if (entity == null) throw new ExceptionNotFound("Not found");
            _mapper.Map(req, entity);
            BeforeUpdate(entity, req);
            await _db.SaveChangesAsync();
            return _mapper.Map<TResponse>(entity);
        }

        public virtual async Task<TResponse> Delete(int id)
        {
            var set = _db.Set<TEntity>();
            var entity = await set.FindAsync(id);
            if (entity == null) throw new ExceptionNotFound("Not found");
            set.Remove(entity);
            await _db.SaveChangesAsync();
            return null;
        }

        public async virtual Task BeforeCreate(TEntity entity, TCreate req)
        {
        }

        public async virtual Task BeforeUpdate(TEntity entity, TUpdate req)
        {
        }

        public async virtual Task BeforeDelete(TEntity entity, int id)
        {
        }
    }
}
