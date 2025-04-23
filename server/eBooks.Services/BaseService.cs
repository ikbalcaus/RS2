using eBooks.Database;
using eBooks.Models;
using MapsterMapper;

namespace eBooks.Services
{
    public class BaseService<TModel, TSearch, TDbEntity> : IBaseService<TModel, TSearch> where TSearch : BaseSearchObject where TDbEntity : class where TModel : class
    {
        public EBooksContext _db { get; set; }
        public IMapper _mapper { get; set; }
        public BaseService(EBooksContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public PagedResult<TModel> GetPaged(TSearch search)
        {
            List<TModel> result = new List<TModel>();

            var query = _db.Set<TDbEntity>().AsQueryable();

            query = AddFilter(search, query);

            int count = query.Count();

            if (search?.Page.HasValue == true && search?.PageSize.HasValue == true)
            {
                query = query.Skip(search.Page.Value * search.PageSize.Value).Take(search.PageSize.Value);
            }

            var list = query.ToList();

            result = _mapper.Map(list, result);

            PagedResult<TModel> pagedResult = new PagedResult<TModel>();
            pagedResult.ResultList = result;
            pagedResult.Count = count;

            return pagedResult;
        }

        public virtual IQueryable<TDbEntity> AddFilter(TSearch search, IQueryable<TDbEntity> query)
        {
            return query;
        }

        public TModel GetById(int id)
        {
            var entity = _db.Set<TDbEntity>().Find(id);

            if (entity != null)
            {
                return _mapper.Map<TModel>(entity);
            }
            else
            {
                return null;
            }
        }
    }
}
