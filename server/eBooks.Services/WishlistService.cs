using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Interfaces;
using eBooks.Models.Responses;
using MapsterMapper;
using Microsoft.AspNetCore.Http;

namespace eBooks.Services
{
    public class WishlistService : BaseUserContextService<Wishlist, WishlistRes>, IWishlistService
    {
        public WishlistService(EBooksContext db, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            : base(db, mapper, httpContextAccessor)
        {
        }
    }
}
