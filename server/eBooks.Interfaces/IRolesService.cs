using eBooks.Models.Responses;
using eBooks.Models.Search;
using eBooks.Services;

namespace eBooks.Interfaces
{
    public interface IRolesService : IBaseReadOnlyService<BaseSearch, RolesRes>
    {
        Task<UsersRes> AssignRole(int userId, int roleId);
    }
}
