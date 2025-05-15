using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Interfaces;
using eBooks.Models.Requests;
using eBooks.Models.Responses;
using eBooks.Models.SearchObjects;
using MapsterMapper;

namespace eBooks.Services
{
    public class GenresService : BaseCRUDService<Genre, BaseSearch, GenresReq, GenresReq, GenresRes>, IGenresService
    {
        public GenresService(EBooksContext db, IMapper mapper)
            : base(db, mapper)
        {
        }
    }
}
