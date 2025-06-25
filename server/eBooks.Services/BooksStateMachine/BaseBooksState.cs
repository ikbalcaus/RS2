using System.Security.Claims;
using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Models.Exceptions;
using eBooks.Models.Requests;
using eBooks.Models.Responses;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace eBooks.Services.BooksStateMachine
{
    public class BaseBooksState
    {
        protected EBooksContext _db;
        protected IMapper _mapper;
        protected IHttpContextAccessor _httpContextAccessor;
        protected IServiceProvider _serviceProvider;
        protected ILogger<BooksService> _logger;

        public BaseBooksState(EBooksContext db, IMapper mapper, IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider, ILogger<BooksService> logger)
        {
            _db = db;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected int GetUserId() => int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : 0;


        public virtual async Task<BooksRes> Approve(int id)
        {
            throw new ExceptionForbidden("Method not allowed");
        }

        public virtual async Task<BooksRes> Await(int id)
        {
            throw new ExceptionForbidden("Method not allowed");
        }

        public virtual async Task<BooksRes> Update(int id, BooksPutReq req)
        {
            throw new ExceptionForbidden("Method not allowed");
        }

        public virtual async Task<BooksRes> Hide(int id)
        {
            throw new ExceptionForbidden("Method not allowed");
        }

        public virtual async Task<BooksRes> Reject(int id, string message)
        {
            throw new ExceptionForbidden("Method not allowed");
        }

        public virtual List<string> AdminAllowedActions()
        {
            throw new ExceptionForbidden("Method not allowed");
        }

        public virtual List<string> UserAllowedActions()
        {
            throw new ExceptionForbidden("Method not allowed");
        }

        public BaseBooksState CheckState(string state)
        {
            return state switch
            {
                "approve" => _serviceProvider.GetService<ApproveBooksState>(),
                "await" => _serviceProvider.GetService<AwaitBooksState>(),
                "draft" => _serviceProvider.GetService<DraftBooksState>(),
                "hide" => _serviceProvider.GetService<HideBooksState>(),
                "reject" => _serviceProvider.GetService<RejectBooksState>(),
                _ => throw new Exception("State not recognized"),
            };
        }
    }
}
