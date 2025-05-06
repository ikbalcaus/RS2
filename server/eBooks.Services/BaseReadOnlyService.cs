using eBooks.Database;
using eBooks.Models;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace eBooks.Services
{
    public class BaseReadOnlyService<TSearch, TEntity, TResponse> : IBaseReadOnlyService<TSearch, TResponse>
        where TSearch : BaseSearch
        where TEntity : class
        where TResponse : class
    {
        protected EBooksContext _db;
        protected IMapper _mapper;

        public BaseReadOnlyService(EBooksContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<PagedResult<TResponse>> GetPaged(TSearch search)
        {
            List<TResponse> result = new List<TResponse>();
            var query = _db.Set<TEntity>().AsQueryable();
            query = await AddFilter(search, query);
            int count = await query.CountAsync();
            if (search?.Page.HasValue == true && search?.PageSize.HasValue == true)
            {
                query = query.Skip(search.Page.Value * search.PageSize.Value).Take(search.PageSize.Value);
            }
            var list = await query.ToListAsync();
            result = _mapper.Map(list, result);
            PagedResult<TResponse> pagedResult = new PagedResult<TResponse>
            {
                ResultList = result,
                Count = count
            };
            return pagedResult;
        }

        public async Task<TResponse> GetById(int id)
        {
            var entity = await _db.Set<TEntity>().FindAsync(id);
            if (entity == null) return null;
            else return _mapper.Map<TResponse>(entity);
        }

        public async virtual Task<IQueryable<TEntity>> AddFilter(TSearch search, IQueryable<TEntity> query)
        {
            return query;
        }
    }
}
