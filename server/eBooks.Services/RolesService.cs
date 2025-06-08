using System.Security.Claims;
using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Interfaces;
using eBooks.Models.Exceptions;
using eBooks.Models.Responses;
using eBooks.Models.Search;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace eBooks.Services
{
    public class RolesService : BaseReadOnlyService<Role, BaseSearch, RolesRes>, IRolesService
    {
        protected IHttpContextAccessor _httpContextAccessor;

        public RolesService(EBooksContext db, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            : base(db, mapper)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<UsersRes> AssignRole(int userId, int roleId)
        {
            var entity = await _db.Set<User>().Include(x => x.Role).FirstOrDefaultAsync(x => x.UserId == userId);
            if (entity == null)
                throw new ExceptionNotFound();
            if (!await _db.Set<Role>().AnyAsync(x => x.RoleId == roleId))
                throw new ExceptionNotFound();
            if (userId == (int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : 0))
                throw new ExceptionBadRequest("You cannot assign role to yourself");
            entity.RoleId = roleId;
            await _db.SaveChangesAsync();
            return _mapper.Map<UsersRes>(entity);
        }
    }
}
