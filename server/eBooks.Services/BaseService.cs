using eBooks.Database;
using eBooks.Interfaces;
using eBooks.Models;
using MapsterMapper;
using Microsoft.Extensions.Logging;

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
            BeforeCreate(req, entity);
            _db.Add(entity);
            _db.SaveChanges();
            return _mapper.Map<TResponse>(entity);
        }

        public virtual TResponse Update(int id, TUpdate req)
        {
            var set = _db.Set<TEntity>();
            var entity = set.Find(id);
            _mapper.Map(req, entity);
            BeforeUpdate(req, entity);
            _db.SaveChanges();
            return _mapper.Map<TResponse>(entity);
        }

        public virtual void Delete(int id)
        {
            var set = _db.Set<TEntity>();
            var entity = set.Find(id);
            set.Remove(entity);
            _db.SaveChanges();
        }

        public virtual void BeforeCreate(TCreate request, TEntity entity)
        {
        }

        public virtual void BeforeUpdate(TUpdate request, TEntity entity)
        {
        }
    }
}
