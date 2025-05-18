using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Interfaces;
using eBooks.Models.Exceptions;
using eBooks.Models.Responses;
using eBooks.Models.SearchObjects;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace eBooks.Services
{
    public class RolesService : BaseReadOnlyService<Role, BaseSearch, RolesRes>, IRolesService
    {
        public RolesService(EBooksContext db, IMapper mapper)
            : base(db, mapper)
        {
        }

        public async Task<UsersRes> AssignRole(int userId, int roleId)
        {
            var entity = await _db.Set<User>().Include(x => x.Role).FirstOrDefaultAsync(x => x.UserId == userId);
            if (entity == null)
                throw new ExceptionNotFound();
            var role = await _db.Set<Role>().FirstOrDefaultAsync(x => x.RoleId == roleId);
            if (role == null)
                throw new ExceptionNotFound();
            entity.Role = role;
            await _db.SaveChangesAsync();
            return _mapper.Map<UsersRes>(entity);
        }
    }
}
