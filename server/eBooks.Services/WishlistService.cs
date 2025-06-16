using Azure;
using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Interfaces;
using eBooks.Models.Exceptions;
using eBooks.Models.Responses;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace eBooks.Services
{
    public class WishlistService : BaseUserContextService<Wishlist, object, WishlistRes>, IWishlistService
    {
        public WishlistService(EBooksContext db, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            : base(db, mapper, httpContextAccessor, false)
        {
        }

        public override async Task<WishlistRes> Post(int bookId, object req)
        {
            var set = _db.Set<Wishlist>();
            if (!await _db.Set<Book>().AnyAsync(x => x.BookId == bookId))
                throw new ExceptionNotFound();
            var userId = GetUserId();
            if (await set.AnyAsync(x => x.UserId == userId && x.BookId == bookId))
                throw new ExceptionBadRequest("Already exist");
            var entity = new Wishlist
            {
                UserId = userId,
                BookId = bookId,
                ModifiedAt = DateTime.UtcNow
            };
            set.Add(entity);
            await _db.SaveChangesAsync();
            return _mapper.Map<WishlistRes>(entity);
        }
    }
}
