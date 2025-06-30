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
    public class WishlistService : BaseUserContextService<Wishlist, BaseSearch, object, WishlistRes>, IWishlistService
    {
        public WishlistService(EBooksContext db, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            : base(db, mapper, httpContextAccessor, false)
        {
        }

        public override IQueryable<Wishlist> AddIncludes(IQueryable<Wishlist> query, BaseSearch? search = null)
        {
            query = query.Include(x => x.Book);
            return query;
        }
    }
}
