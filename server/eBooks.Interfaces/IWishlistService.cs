using System;
using eBooks.Models.Responses;
using eBooks.Models.Search;

namespace eBooks.Interfaces
{
    public interface IWishlistService : IBaseUserContextService<BaseSearch, object, WishlistRes>
    {
    }
}
