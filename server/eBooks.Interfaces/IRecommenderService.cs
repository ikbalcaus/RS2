using eBooks.Models.Responses;

namespace eBooks.Interfaces
{
    public interface IRecommenderService
    {
        Task TrainModel(int numberOfIterations, int approximationRank, float alpha, float lambda, int batchIterations = 10, int patience = 3);
        Task LoadModel();
        Task<List<BooksRes>> GetRecommendedBooks(int bookId, int numberOfRecommendations);
    }
}
