using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Interfaces;
using eBooks.Models.Exceptions;
using eBooks.Models.Requests;
using eBooks.Models.Responses;
using eBooks.Models.Search;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace eBooks.Services
{
    public class ReportsService : BaseUserContextService<Report, ReportsSearch, ReportsReq, ReportsRes>, IReportsService
    {
        public ReportsService(EBooksContext db, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            : base(db, mapper, httpContextAccessor, false)
        {
        }

        public override IQueryable<Report> AddIncludes(IQueryable<Report> query, ReportsSearch? search = null)
        {
            query = query.Include(x => x.User);
            if (search == null || search.IsBookIncluded == true)
                query = query.Include(x => x.Book);
            return query;
        }

        public override IQueryable<Report> AddFilters(IQueryable<Report> query, ReportsSearch search)
        {
            if (!string.IsNullOrWhiteSpace(search.ReportedBy))
                query = query.Where(x => x.User.UserName.ToLower().Contains(search.ReportedBy.ToLower()));
            if (!string.IsNullOrWhiteSpace(search.BookTitle))
                query = query.Where(x => x.Book.Title.ToLower().Contains(search.BookTitle.ToLower()));
            if (search.OrderBy == "First added")
                query = query.OrderBy(x => x.ModifiedAt);
            else
                query = query.OrderByDescending(x => x.ModifiedAt);
            return query;
        }

        public override async Task<PagedResult<ReportsRes>> GetPaged(ReportsSearch search)
        {
            var result = new List<ReportsRes>();
            var query = _db.Set<Report>().AsQueryable();
            query = AddIncludes(query, search);
            query = AddFilters(query, search);
            int count = await query.CountAsync();
            if (search?.Page.HasValue == true && search?.PageSize.HasValue == true && search.Page.Value > 0)
                query = query.Skip((search.Page.Value - 1) * search.PageSize.Value).Take(search.PageSize.Value);
            var list = await query.ToListAsync();
            result = _mapper.Map(list, result);
            PagedResult<ReportsRes> pagedResult = new PagedResult<ReportsRes>
            {
                ResultList = result,
                Count = count
            };
            return pagedResult;
        }

        public async Task<ReportsRes> AdminDelete(int userId, int bookId)
        {
            var set = _db.Set<Report>();
            var entity = await set.FindAsync(userId, bookId);
            if (entity == null)
                throw new ExceptionNotFound();
            set.Remove(entity);
            await _db.SaveChangesAsync();
            return null;
        }
    }
}
