using eBooks.Interfaces;
using eBooks.Models.Wishlist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eBooks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WishlistController
    {
        protected IWishlistService _service;

        public WishlistController(IWishlistService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Policy = "User")]
        public async Task<List<WishlistRes>> Get()
        {
            return await _service.Get();
        }

        [HttpPost("/{bookId}")]
        [Authorize(Policy = "User")]
        public async Task<WishlistRes> Post(int bookId)
        {
            return await _service.Post(bookId);
        }

        [HttpPatch("/{bookId}")]
        [Authorize(Policy = "User")]
        public async Task<WishlistRes> Patch(int bookId)
        {
            return await _service.Patch(bookId);
        }

        [HttpDelete("/{bookId}")]
        [Authorize(Policy = "User")]
        public async Task<WishlistRes> Delete(int bookId)
        {
            return await _service.Delete(bookId);
        }
    }
}
