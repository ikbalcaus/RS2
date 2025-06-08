using eBooks.Models.Requests;
using eBooks.Models.Responses;
using eBooks.Models.Search;
using eBooks.Services;

namespace eBooks.Interfaces
{
    public interface IQuestionsService : IBaseReadOnlyService<QuestionsSearch, QuestionsRes>
    {
        Task<QuestionsRes> Post(QuestionsReq req);
        Task<QuestionsRes> Patch(int id, QuestionsReq req);
    }
}
