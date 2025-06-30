using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Interfaces;
using eBooks.Models.Requests;
using eBooks.Models.Responses;
using eBooks.Models.Search;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace eBooks.Services
{
    public class AuthorsService : BaseCRUDService<Author, AuthorsSearch, AuthorsReq, AuthorsReq, AuthorsRes>, IAuthorsService
    {
        public AuthorsService(EBooksContext db, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            : base(db, mapper, httpContextAccessor)
        {
        }

        public override IQueryable<Author> AddIncludes(IQueryable<Author> query, AuthorsSearch? search = null)
        {
            query = query.Include(x => x.ModifiedBy);
            return query;
        }

        public override IQueryable<Author> AddFilters(IQueryable<Author> query, AuthorsSearch search)
        {
            if (!string.IsNullOrWhiteSpace(search.Name))
                query = query.Where(x => x.Name.ToLower().Contains(search.Name.ToLower()));
            query = search.OrderBy switch
            {
                "First modified" => query.OrderBy(x => x.ModifiedAt),
                "Name (A-Z)" => query.OrderBy(x => x.Name),
                "Name (Z-A)" => query.OrderByDescending(x => x.Name),
                _ => query.OrderByDescending(x => x.ModifiedAt),
            };
            return query;
        }

        public override void BeforeSaveChanges(Author entity)
        {
            entity.ModifiedAt = DateTime.UtcNow;
            entity.ModifiedById = GetUserId();
        }
    }
}
