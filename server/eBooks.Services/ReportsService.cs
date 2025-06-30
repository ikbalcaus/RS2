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
