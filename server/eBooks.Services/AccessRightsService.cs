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
    public class AccessRightsService : BaseUserContextService<AccessRight, object, AccessRightsRes>, IAccessRightsService
    {
        public AccessRightsService(EBooksContext db, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            : base(db, mapper, httpContextAccessor, false)
        {
        }

        public override async Task<AccessRightsRes> Post(int bookId, object req)
        {
            var errors = new Dictionary<string, List<string>>();
            var book = await _db.Set<Book>().FindAsync(bookId);
            if (book == null)
                throw new ExceptionNotFound();
            var userId = GetUserId();
            if (await _db.Set<AccessRight>().AnyAsync(x => x.UserId == userId && x.BookId == bookId))
                errors.AddError("Book", "You already possess this book");
            if (book.Price == 0)
                errors.AddError("Book", "This book is free, you cannot buy it");
            if (userId == book.PublisherId)
                errors.AddError("Book", "You cannot add your own book");
            if (book.StateMachine != "approve")
                errors.AddError("Book", "This book is not active right now");
            if (errors.Count > 0)
                throw new ExceptionBadRequest(errors);
            if (errors.Count > 0)
                throw new ExceptionBadRequest(errors);
            var entity = new AccessRight
            {
                UserId = userId,
                BookId = bookId
            };
            var wishlistItem = await _db.Set<Wishlist>().FindAsync(userId, bookId);
            _db.Set<AccessRight>().Add(entity);
            if (wishlistItem != null)
                _db.Set<Wishlist>().Remove(wishlistItem);
            await _db.SaveChangesAsync();
            return _mapper.Map<AccessRightsRes>(entity);
        }

        public override async Task<AccessRightsRes> Delete(int bookId)
        {
            var userId = GetUserId();
            var entity = await _db.Set<AccessRight>().FirstOrDefaultAsync(x => x.UserId == userId && x.BookId == bookId);
            if (entity == null)
                throw new ExceptionNotFound();
            entity.Hidden = !entity.Hidden;
            return null;
        }
    }
}
