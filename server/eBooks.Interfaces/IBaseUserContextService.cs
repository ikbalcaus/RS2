namespace eBooks.Interfaces
{
    public interface IBaseUserContextService<TResponse>
    {
        Task<List<TResponse>> GetAll();
        Task<TResponse> GetByBookId(int bookId);
        Task<TResponse> Post(int bookId);
        Task<TResponse> Patch(int bookId);
        Task<TResponse> Delete(int bookId);
    }
}
