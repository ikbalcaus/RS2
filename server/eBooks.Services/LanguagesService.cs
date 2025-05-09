using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Interfaces;
using eBooks.Models;
using eBooks.Models.Languages;
using MapsterMapper;

namespace eBooks.Services
{
    public class LanguagesService : BaseService<Language, BaseSearch, LanguagesCreateReq, LanguagesUpdateReq, LanguagesRes>, ILanguagesService
    {
        public LanguagesService(EBooksContext db, IMapper mapper)
            : base(db, mapper)
        {
        }
    }
}
