using eBooks.Models;
using eBooks.Models.Authors;

namespace eBooks.Interfaces
{
    public interface IAuthorsService : IBaseService<BaseSearch, AuthorsCreateReq, AuthorsUpdateReq, AuthorsRes>
    {
    }
}
