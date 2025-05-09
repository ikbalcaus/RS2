using eBooks.Models;
using eBooks.Models.Genres;

namespace eBooks.Interfaces
{
    public interface IGenresService : IBaseService<BaseSearch, GenresReq, GenresReq, GenresRes>
    {
    }
}
