using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Interfaces;
using eBooks.Models.Requests;
using eBooks.Models.Responses;
using eBooks.Models.SearchObjects;
using MapsterMapper;

namespace eBooks.Services
{
    public class AuthorsService : BaseCRUDService<Author, BaseSearch, AuthorsPostReq, AuthorsPutReq, AuthorsRes>, IAuthorsService
    {
        public AuthorsService(EBooksContext db, IMapper mapper)
            : base(db, mapper)
        {
        }
    }
}
