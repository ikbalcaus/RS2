using System.Security.Claims;
using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Interfaces;
using eBooks.Models.AccessRights;
using eBooks.Models.Exceptions;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace eBooks.Services
{
    public class AccessRightsService : IAccessRightsService
    {
        protected EBooksContext _db;
        protected IMapper _mapper;
        protected IHttpContextAccessor _httpContextAccessor;

        public AccessRightsService(EBooksContext db, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<AccessRightsRes>> GetAll()
        {
            var set = _db.Set<AccessRight>();
            var userId = int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var temp) ? temp : throw new ExceptionForbidden("User not logged in");
            var entity = await set.Where(x => x.UserId == userId).Include(x => x.Book).ToListAsync();
            return _mapper.Map<List<AccessRightsRes>>(entity);
        }

        public async Task<AccessRightsRes> GetById(int bookId)
        {
            var set = _db.Set<AccessRight>();
            var userId = int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var temp) ? temp : throw new ExceptionForbidden("User not logged in");
            var entity = await set.Where(x => x.UserId == userId && x.BookId == bookId).Include(x => x.Book).FirstOrDefaultAsync();
            if (entity == null)
                throw new ExceptionNotFound();
            return _mapper.Map<AccessRightsRes>(entity);
        }

        public async Task<AccessRightsRes> Post(int bookId)
        {
            var book = await _db.Set<Book>().FindAsync(bookId);
            if (book == null)
                throw new ExceptionNotFound();
            if (book.Price > 0)
                throw new ExceptionForbidden("This book is not free");
            var set = _db.Set<AccessRight>();
            var entity = new AccessRight()
            {
                UserId = int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var temp) ? temp : throw new ExceptionForbidden("User not logged in"),
                BookId = bookId
            };
            set.Add(entity);
            await _db.SaveChangesAsync();
            return _mapper.Map<AccessRightsRes>(entity);
        }

        public async Task<AccessRightsRes> Patch(int bookId)
        {
            var set = _db.Set<AccessRight>();
            var userId = int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var temp) ? temp : throw new ExceptionForbidden("User not logged in");
            var entity = await set.FirstOrDefaultAsync(x => x.UserId == userId && x.BookId == bookId);
            entity.ModifiedAt = DateTime.Now;
            await _db.SaveChangesAsync();
            return _mapper.Map<AccessRightsRes>(entity);
        }

        public async Task<AccessRightsRes> Delete(int bookId)
        {
            var set = _db.Set<AccessRight>();
            var userId = int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var temp) ? temp : throw new ExceptionForbidden("User not logged in");
            var entity = await set.FirstOrDefaultAsync(x => x.UserId == userId && x.BookId == bookId);
            entity.Hidden = !entity.Hidden;
            await _db.SaveChangesAsync();
            return null;
        }
    }
}
