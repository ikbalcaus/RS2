using eBooks.API.Auth;
using eBooks.Interfaces;
using eBooks.Models.Responses;
using eBooks.Models.Requests;
using eBooks.Models.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eBooks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BooksController : BaseCRUDController<BooksSearch, BooksPostReq, BooksPutReq, BooksRes>
    {
        protected new IBooksService _service;
        protected AccessControlHandler _accessControlHandler;

        public BooksController(IBooksService service, AccessControlHandler accessControlHandler)
            : base(service)
        {
            _service = service;
            _accessControlHandler = accessControlHandler;
        }

        [AllowAnonymous]
        public override async Task<PagedResult<BooksRes>> GetPaged([FromQuery] BooksSearch search)
        {
            return await base.GetPaged(search);
        }

        [AllowAnonymous]
        public override async Task<BooksRes> GetById(int id)
        {
            return await base.GetById(id);
        }

        [Authorize(Policy = "User")]
        public override async Task<BooksRes> Post(BooksPostReq req)
        {
            return await base.Post(req);
        }

        [Authorize(Policy = "User")]
        public override async Task<BooksRes> Put(int id, BooksPutReq req)
        {
            await _accessControlHandler.CheckIsOwnerByBookId(id);
            return await base.Put(id, req);
        }

        [Authorize(Policy = "User")]
        public override async Task<BooksRes> Delete(int id)
        {
            await _accessControlHandler.CheckIsOwnerByBookId(id);
            return await base.Delete(id);
        }

        [Authorize(Policy = "Admin")]
        [HttpDelete("{id}/admin-delete")]
        public async Task<BooksRes> DeleteByAdmin(int id, string reason)
        {
            return await _service.DeleteByAdmin(id, reason);
        }

        [Authorize(Policy = "Admin")]
        [HttpPatch("{id}/undo-delete")]
        public async Task<BooksRes> UndoDelete(int id)
        {
            return await _service.UndoDelete(id);
        }

        [Authorize(Policy = "User")]
        [HttpPut("{id}/set-discount")]
        public async Task<BooksRes> SetDiscount(int id, DiscountReq req)
        {
            await _accessControlHandler.CheckIsOwnerByBookId(id);
            return await _service.SetDiscount(id, req);
        }

        [Authorize(Policy = "User")]
        [HttpGet("{id}/get-book-file")]
        public async Task<IActionResult> GetBookFile(int id)
        {
            var file = await _service.GetBookFile(id);
            return File(file.Item2, "application/pdf", file.Item1);
        }

        [Authorize(Policy = "User")]
        [HttpPatch("{id}/await")]
        public async Task<BooksRes> Await(int id)
        {
            await _accessControlHandler.CheckIsOwnerByBookId(id);
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
            await _accessControlHandler.CheckIsOwnerByBookId(id);
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
