using eBooks.Models.Requests;
using eBooks.Models.Responses;
using eBooks.Models.SearchObjects;

namespace eBooks.Interfaces
{
    public interface IAuthorsService : IBaseCRUDService<BaseSearch, AuthorsPostReq, AuthorsPutReq, AuthorsRes>
    {
    }
}
