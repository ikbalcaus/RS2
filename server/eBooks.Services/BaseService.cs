using eBooks.Database;
using eBooks.Models;
using MapsterMapper;

namespace eBooks.Services
{
    public abstract class BaseService<TEntity, TSearch, TCreate, TUpdate, TResponse> : BaseReadOnlyService<TSearch, TEntity, TResponse> where TResponse : class where TSearch : BaseSearch where TEntity : class
    {
        public BaseService(EBooksContext db, IMapper mapper) : base(db, mapper)
        {
        }

        public virtual TResponse Create(TCreate req)
        {
            TEntity entity = _mapper.Map<TEntity>(req);
            BeforeCreate(entity, req);
            _db.Add(entity);
            _db.SaveChanges();
            return _mapper.Map<TResponse>(entity);
        }

        public virtual TResponse Update(int id, TUpdate req)
        {
            var set = _db.Set<TEntity>();
            var entity = set.Find(id);
            _mapper.Map(req, entity);
            BeforeUpdate(entity, req);
            _db.SaveChanges();
            return _mapper.Map<TResponse>(entity);
        }

        public virtual TResponse Delete(int id)
        {
            var set = _db.Set<TEntity>();
            var entity = set.Find(id);
            set.Remove(entity);
            _db.SaveChanges();
            return null;
        }

        public virtual void BeforeCreate(TEntity entity, TCreate req)
        {
        }

        public virtual void BeforeUpdate(TEntity entity, TUpdate req)
        {
        }
    }
}
