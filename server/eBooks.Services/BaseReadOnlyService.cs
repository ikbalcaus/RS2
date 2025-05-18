using eBooks.Database;
using eBooks.Models;
using eBooks.Models.Exceptions;
using eBooks.Models.SearchObjects;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace eBooks.Services
{
    public abstract class BaseReadOnlyService<TEntity, TSearch, TResponse> : IBaseReadOnlyService<TSearch, TResponse>
        where TEntity : class
        where TSearch : BaseSearch
        where TResponse : class
    {
        protected EBooksContext _db;
        protected IMapper _mapper;

        public BaseReadOnlyService(EBooksContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public virtual async Task<PagedResult<TResponse>> GetPaged(TSearch search)
        {
            var result = new List<TResponse>();
            var query = _db.Set<TEntity>().AsQueryable();
            query = await AddFilter(search, query);
            int count = await query.CountAsync();
            if (search?.Page.HasValue == true && search?.PageSize.HasValue == true)
                query = query.Skip(search.Page.Value * search.PageSize.Value).Take(search.PageSize.Value);
            var list = await query.ToListAsync();
            result = _mapper.Map(list, result);
            PagedResult<TResponse> pagedResult = new PagedResult<TResponse>
            {
                ResultList = result,
                Count = count
            };
            return pagedResult;
        }

        public virtual async Task<TResponse> GetById(int id)
        {
            var entity = await _db.Set<TEntity>().FindAsync(id);
            if (entity == null)
                throw new ExceptionNotFound();
            return _mapper.Map<TResponse>(entity);
        }

        public virtual async Task<IQueryable<TEntity>> AddFilter(TSearch search, IQueryable<TEntity> query)
        {
            return query;
        }
    }
}
