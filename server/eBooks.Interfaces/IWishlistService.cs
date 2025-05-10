using eBooks.Models.Wishlist;

namespace eBooks.Interfaces
{
    public interface IWishlistService
    {
        Task<List<WishlistRes>> Get();
        Task<WishlistRes> Post(int bookId);
        Task<WishlistRes> Patch(int bookId);
        Task<WishlistRes> Delete(int bookId);
    }
}
