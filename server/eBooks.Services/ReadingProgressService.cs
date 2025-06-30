using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Interfaces;
using eBooks.Models.Responses;
using eBooks.Models.Search;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace eBooks.Services
{
    public class ReadingProgressService : BaseUserContextService<ReadingProgress, BaseSearch, object, ReadingProgressRes>, IReadingProgressService
    {
        public ReadingProgressService(EBooksContext db, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            : base(db, mapper, httpContextAccessor, true)
        {
        }

        public override IQueryable<ReadingProgress> AddIncludes(IQueryable<ReadingProgress> query, BaseSearch? search = null)
        {
            query = query.Include(x => x.Book);
            return query;
        }
    }
}
