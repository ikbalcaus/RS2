using eBooks.Models.Responses;

namespace eBooks.Interfaces
{
    public interface IAccessRightsService : IBaseUserContextService<object, AccessRightsRes>
    {
        Task<AccessRightsRes> ToggleFavorite(int bookId);
    }
}
