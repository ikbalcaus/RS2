using eBooks.Database.Models;
using eBooks.Database;
using eBooks.Interfaces;
using eBooks.Models.Responses;
using MapsterMapper;
using Microsoft.AspNetCore.Http;

namespace eBooks.Services
{
    public class FavoritesService : BaseUserContextService<Favorite, FavoritesRes>, IFavoritesService
    {
        public FavoritesService(EBooksContext db, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            : base(db, mapper, httpContextAccessor)
        {
        }
    }
}
