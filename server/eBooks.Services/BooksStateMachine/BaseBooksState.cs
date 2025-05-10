using System.Runtime.InteropServices;
using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Models;
using eBooks.Models.Books;
using eBooks.Models.Exceptions;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace eBooks.Services.BooksStateMachine
{
    public class BaseBooksState
    {
        protected EBooksContext _db { get; set; }
        protected IMapper _mapper { get; set; }
        protected IServiceProvider _serviceProvider { get; set; }

        public BaseBooksState(EBooksContext db, IMapper mapper, IServiceProvider serviceProvider)
        {
            _db = db;
            _mapper = mapper;
            _serviceProvider = serviceProvider;
        }

        public virtual async Task<BooksRes> Update(int id, BooksUpdateReq req)
        {
            throw new ExceptionForbidden("Method not allowed");
        }

        public virtual async Task<BooksRes> Await(int id)
        {
            throw new ExceptionForbidden("Method not allowed");
        }

        public virtual async Task<BooksRes> Approve(int id)
        {
            throw new ExceptionForbidden("Method not allowed");
        }

        public virtual async Task<BooksRes> Reject(int id, string message)
        {
            throw new ExceptionForbidden("Method not allowed");
        }

        public virtual async Task<BooksRes> Hide(int id)
        {
            throw new ExceptionForbidden("Method not allowed");
        }

        public virtual async Task<List<string>> AllowedActions(Book entity)
        {
            throw new ExceptionForbidden("Method not allowed");
        }

        public async Task<BaseBooksState> CheckState(string state)
        {
            switch (state)
            {
                case "draft":
                    return _serviceProvider.GetService<DraftBooksState>();
                case "await":
                    return _serviceProvider.GetService<AwaitBooksState>();
                case "approve":
                    return _serviceProvider.GetService<ApproveBooksState>();
                case "reject":
                    return _serviceProvider.GetService<RejectBooksState>();
                case "archive":
                    return _serviceProvider.GetService<HideBooksState>();
                default:
                    throw new ExceptionBadRequest("State not recognized");
            }
        }
    }
}
