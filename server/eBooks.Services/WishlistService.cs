using System.Security.Claims;
using Azure;
using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Interfaces;
using eBooks.Models;
using eBooks.Models.Exceptions;
using eBooks.Models.Wishlist;
using eBooks.Services.BooksStateMachine;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace eBooks.Services
{
    public class WishlistService : IWishlistService
    {
        protected EBooksContext _db;
        protected IMapper _mapper;
        protected IHttpContextAccessor _httpContextAccessor;

        public WishlistService(EBooksContext db, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<WishlistRes>> Get()
        {
            var set = _db.Set<Wishlist>();
            var userId = int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var temp) ? temp : throw new ExceptionForbidden("User not logged in");
            var entity = await set.Where(x => x.UserId == userId).ToListAsync();
            if (entity == null)
                throw new ExceptionNotFound();
            return _mapper.Map<List<WishlistRes>>(entity);
        }

        public async Task<WishlistRes> Post(int bookId)
        {
            var set = _db.Set<Wishlist>();
            var entity = new Wishlist()
            {
                UserId = int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var temp) ? temp : throw new ExceptionForbidden("User not logged in"),
                BookId = bookId
            };
            set.Add(entity);
            await _db.SaveChangesAsync();
            return _mapper.Map<WishlistRes>(entity);
        }

        public async Task<WishlistRes> Patch(int bookId)
        {
            var set = _db.Set<Wishlist>();
            var userId = int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var temp) ? temp : throw new ExceptionForbidden("User not logged in");
            var entity = await set.FirstOrDefaultAsync(x => x.UserId == userId && x.BookId == bookId);
            entity.ModifiedAt = DateTime.Now;
            await _db.SaveChangesAsync();
            return _mapper.Map<WishlistRes>(entity);
        }

        public async Task<WishlistRes> Delete(int bookId)
        {
            var set = _db.Set<Wishlist>();
            var userId = int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var temp) ? temp : throw new ExceptionForbidden("User not logged in");
            var entity = await set.FirstOrDefaultAsync(x => x.UserId == userId && x.BookId == bookId);
            set.Remove(entity);
            await _db.SaveChangesAsync();
            return null;
        }
    }
}
