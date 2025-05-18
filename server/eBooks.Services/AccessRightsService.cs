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
            var book = await _db.Set<Book>().FindAsync(bookId);
            if (book == null)
                throw new ExceptionNotFound();
            if (await AccessRightExist(bookId))
                throw new ExceptionBadRequest("You already possess this book");
            if (book.Price > 0)
                throw new ExceptionBadRequest("This book is not free");
            var userId = GetUserId();
            var entity = new AccessRight
            {
                UserId = userId,
                BookId = bookId
            };
            _db.Set<AccessRight>().Add(entity);
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
