using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Interfaces;
using eBooks.Models.Requests;
using eBooks.Models.Responses;
using eBooks.Models.SearchObjects;
using MapsterMapper;

namespace eBooks.Services
{
    public class LanguagesService : BaseCRUDService<Language, BaseSearch, LanguagesPostReq, LanguagesPutReq, LanguagesRes>, ILanguagesService
    {
        public LanguagesService(EBooksContext db, IMapper mapper)
            : base(db, mapper)
        {
        }
    }
}
