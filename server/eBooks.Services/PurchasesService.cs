using eBooks.Database.Models;
using eBooks.Database;
using eBooks.Interfaces;
using eBooks.Models.Responses;
using eBooks.Models.Search;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace eBooks.Services
{
    public class PurchasesService : BaseReadOnlyService<Purchase, PurchasesSearch, PurchasesRes>, IPurchasesService
    {
        public PurchasesService(EBooksContext db, IMapper mapper)
            : base(db, mapper)
        {
        }

        public override IQueryable<Purchase> AddIncludes(IQueryable<Purchase> query, PurchasesSearch? search = null)
        {
            query = query.Include(x => x.User);
            query = query.Include(x => x.Publisher);
            query = query.Include(x => x.Book);
            return query;
        }

        public override IQueryable<Purchase> AddFilters(IQueryable<Purchase> query, PurchasesSearch search)
        {
            if (!string.IsNullOrWhiteSpace(search.User))
                query = query.Where(x => x.User.UserName.ToLower().StartsWith(search.User.ToLower()));
            if (!string.IsNullOrWhiteSpace(search.Publisher))
                query = query.Where(x => x.Publisher.UserName.ToLower().StartsWith(search.Publisher.ToLower()));
            if (!string.IsNullOrWhiteSpace(search.Book))
                query = query.Where(x => x.Book.Title.ToLower().StartsWith(search.Book.ToLower()));
            query = search.OrderBy switch
            {
                "First created" => query.OrderBy(x => x.CreatedAt),
                "User (A-Z)" => query.OrderBy(x => x.User.UserName),
                "User (Z-A)" => query.OrderByDescending(x => x.User.UserName),
                "Publisher (A-Z)" => query.OrderBy(x => x.Publisher.UserName),
                "Publisher (Z-A)" => query.OrderByDescending(x => x.Publisher.UserName),
                "Book (A-Z)" => query.OrderByDescending(x => x.Book.Title),
                "Book (Z-A)" => query.OrderByDescending(x => x.Book.Title),
                _ => query.OrderByDescending(x => x.CreatedAt),
            };
            return query;
        }
    }
}
