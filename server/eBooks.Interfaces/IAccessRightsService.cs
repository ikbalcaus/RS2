using eBooks.Models.Responses;
using eBooks.Models.Search;

namespace eBooks.Interfaces
{
    public interface IAccessRightsService : IBaseUserContextService<BaseSearch, object, AccessRightsRes>
    {
        Task<AccessRightsRes> ToggleFavorite(int bookId);
    }
}
