using eBooks.Models;
using eBooks.Models.Languages;

namespace eBooks.Interfaces
{
    public interface ILanguagesService : IBaseService<BaseSearch, LanguagesCreateReq, LanguagesUpdateReq, LanguagesRes>
    {
    }
}
