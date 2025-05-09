using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Interfaces;
using eBooks.Models;
using eBooks.Models.Genres;
using MapsterMapper;

namespace eBooks.Services
{
    public class GenresService : BaseService<Genre, BaseSearch, GenresReq, GenresReq, GenresRes>, IGenresService
    {
        public GenresService(EBooksContext db, IMapper mapper)
            : base(db, mapper)
        {
        }
    }
}
