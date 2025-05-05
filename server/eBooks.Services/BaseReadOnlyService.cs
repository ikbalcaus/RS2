using eBooks.Database;
using eBooks.Models;
using MapsterMapper;

namespace eBooks.Services
{
    public class BaseReadOnlyService<TSearch, TEntity, TResponse> : IBaseReadOnlyService<TSearch, TResponse> where TSearch : BaseSearch where TEntity : class where TResponse : class
    {
        protected EBooksContext _db;
        protected IMapper _mapper;

        public BaseReadOnlyService(EBooksContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public PagedResult<TResponse> GetPaged(TSearch search)
        {
            List<TResponse> result = new List<TResponse>();
            var query = _db.Set<TEntity>().AsQueryable();
            query = AddFilter(search, query);
            int count = query.Count();
            if (search?.Page.HasValue == true && search?.PageSize.HasValue == true)
            {
                query = query.Skip(search.Page.Value * search.PageSize.Value).Take(search.PageSize.Value);
            }
            var list = query.ToList();
            result = _mapper.Map(list, result);
            PagedResult<TResponse> pagedResult = new PagedResult<TResponse>();
            pagedResult.ResultList = result;
            pagedResult.Count = count;
            return pagedResult;
        }

        public TResponse GetById(int id)
        {
            var entity = _db.Set<TEntity>().Find(id);
            if (entity == null) return null;
            else return _mapper.Map<TResponse>(entity);
        }

        public virtual IQueryable<TEntity> AddFilter(TSearch search, IQueryable<TEntity> query)
        {
            return query;
        }
    }
}
