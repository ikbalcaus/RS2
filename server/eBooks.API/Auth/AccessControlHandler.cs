using System.Security.Claims;
using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Models.Exceptions;

namespace eBooks.API.Auth
{
    public class AccessControlHandler
    {
        protected EBooksContext _db;
        protected IHttpContextAccessor _httpContextAccessor;

        public AccessControlHandler(EBooksContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task CheckIsOwnerByUserId(int id)
        {
            int currentUserId = GetCurrentUserId();
            if (currentUserId != id)
                throw new ExceptionForbidden("Only owner can use this action");
        }

        public async Task CheckIsOwnerByBookId(int id)
        {
            int currentUserId = GetCurrentUserId();
            Book book = await GetBookById(id);
            if (currentUserId != book.PublisherId)
                throw new ExceptionForbidden("Only owner can use this action");
        }

        public async Task CheckIsOwnerOrModeratorByBookId(int id)
        {
            int currentUserId = GetCurrentUserId();
            Book book = await GetBookById(id);
            string role = GetCurrentUserRole();
            if (role != "Admin" && role != "Moderator" && currentUserId != book.PublisherId)
                throw new ExceptionForbidden("Only owner or moderator can use this action");
        }

        protected int GetCurrentUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new ExceptionForbidden("User not authenticated");
            return int.Parse(userId);
        }

        protected string GetCurrentUserRole()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var role = user?.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(role))
                throw new ExceptionForbidden("User not authorized");
            return role;
        }

        protected async Task<Book> GetBookById(int id)
        {
            var book = await _db.Set<Book>().FindAsync(id);
            if (book == null)
                throw new ExceptionNotFound();
            return book;
        }
    }
}
