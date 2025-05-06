using eBooks.Interfaces;
using eBooks.Models;
using eBooks.Models.Books;
using eBooks.Models.Exceptions;
using eBooks.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eBooks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BooksController : BaseController<BooksSearch, BooksCreateReq, BooksUpdateReq, BooksRes>
    {
        protected new IBooksService _service;

        public BooksController(IBooksService service, IAuthorizationService authService)
            : base(service, authService)
        {
            _service = service;
        }

        [AllowAnonymous]
        public override async Task<PagedResult<BooksRes>> GetAll([FromQuery] BooksSearch search)
        {
            return await base.GetAll(search);
        }

        [AllowAnonymous]
        public override async Task<BooksRes> GetById(int id)
        {
            return await base.GetById(id);
        }

        [Authorize(Policy = "User")]
        public override async Task<BooksRes> Create(BooksCreateReq req)
        {
            return await base.Create(req);
        }

        [Authorize(Policy = "User")]
        public override async Task<BooksRes> Update(int id, BooksUpdateReq req)
        {
            if (!(await _authService.AuthorizeAsync(User, id, "Owner")).Succeeded)
                throw new ExceptionForbidden("Only owner can use this action");
            return await base.Update(id, req);
        }

        [Authorize(Policy = "User")]
        public override async Task<BooksRes> Delete(int id)
        {
            if (!(await _authService.AuthorizeAsync(User, id, "Owner")).Succeeded)
                throw new ExceptionForbidden("Only owner can use this action");
            return await base.Delete(id);
        }

        [Authorize(Policy = "User")]
        [HttpDelete("{id}/delete-image/{imageId}")]
        public async Task<BookImageRes> DeleteImage(int id, int imageId)
        {
            if (!(await _authService.AuthorizeAsync(User, id, "Owner")).Succeeded)
                throw new ExceptionForbidden("Only owner can use this action");
            return await _service.DeleteImage(id, imageId);
        }

        [Authorize(Policy = "User")]
        [HttpPatch("{id}/await")]
        public async Task<BooksRes> Await(int id)
        {
            if (!(await _authService.AuthorizeAsync(User, id, "Owner")).Succeeded)
                throw new ExceptionForbidden("Only owner can use this action");
            return await _service.Await(id);
        }

        [Authorize(Policy = "Moderator")]
        [HttpPatch("{id}/approve")]
        public async Task<BooksRes> Approve(int id)
        {
            return await _service.Approve(id);
        }

        [Authorize(Policy = "Moderator")]
        [HttpPatch("{id}/reject")]
        public async Task<BooksRes> Reject(int id, string message)
        {
            return await _service.Reject(id, message);
        }

        [Authorize(Policy = "User")]
        [HttpPatch("{id}/hide")]
        public async Task<BooksRes> Hide(int id)
        {
            if (!(await _authService.AuthorizeAsync(User, id, "Owner")).Succeeded)
                throw new ExceptionForbidden("Only owner can use this action");
            return await _service.Hide(id);
        }

        [AllowAnonymous]
        [HttpGet("{id}/allowed-actions")]
        public async Task<List<string>> AllowedActions(int id)
        {
            return await _service.AllowedActions(id);
        }
    }
}
