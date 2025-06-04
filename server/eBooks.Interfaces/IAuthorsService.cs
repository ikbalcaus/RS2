using eBooks.Models.Requests;
using eBooks.Models.Responses;
using eBooks.Models.Search;

namespace eBooks.Interfaces
{
    public interface IAuthorsService : IBaseCRUDService<AuthorsSearch, AuthorsReq, AuthorsReq, AuthorsRes>
    {
    }
}
