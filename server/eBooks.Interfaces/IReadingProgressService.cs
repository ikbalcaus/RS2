using eBooks.Models.Responses;
using eBooks.Models.Search;

namespace eBooks.Interfaces
{
    public interface IReadingProgressService : IBaseUserContextService<BaseSearch, object, ReadingProgressRes>
    {
    }
}
