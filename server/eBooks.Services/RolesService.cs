using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Interfaces;
using eBooks.Models;
using eBooks.Models.Roles;
using MapsterMapper;

namespace eBooks.Services
{
    public class RolesService : BaseReadOnlyService<Role, BaseSearch, RolesRes>, IRolesService
    {
        public RolesService(EBooksContext db, IMapper mapper)
            : base(db, mapper)
        {
        }
    }
}
