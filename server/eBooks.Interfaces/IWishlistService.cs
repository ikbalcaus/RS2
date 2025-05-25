using System;
using eBooks.Models.Responses;

namespace eBooks.Interfaces
{
    public interface IWishlistService : IBaseUserContextService<object, WishlistRes>
    {
    }
}
