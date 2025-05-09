using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Interfaces;
using eBooks.Models;
using eBooks.Models.Authors;
using MapsterMapper;

namespace eBooks.Services
{
    public class AuthorsService : BaseService<Author, BaseSearch, AuthorsCreateReq, AuthorsUpdateReq, AuthorsRes>, IAuthorsService
    {
        public AuthorsService(EBooksContext db, IMapper mapper)
            : base(db, mapper)
        {
        }
    }
}
