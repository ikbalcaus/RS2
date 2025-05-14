using eBooks.Models.AccessRights;

namespace eBooks.Interfaces
{
    public interface IAccessRightsService
    {
        Task<List<AccessRightsRes>> GetAll();
        Task<AccessRightsRes> GetById(int bookId);
        Task<AccessRightsRes> Post(int bookId);
        Task<AccessRightsRes> Patch(int bookId);
        Task<AccessRightsRes> Delete(int bookId);
    }
}
