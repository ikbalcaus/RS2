using eBooks.Database;
using eBooks.Interfaces;
using eBooks.Models.Books;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace eBooks.Services.BooksStateMachine
{
    public class BaseBooksState : IBooksState
    {
        public EBooksContext _db { get; set; }
        public IMapper _mapper { get; set; }
        public IServiceProvider _serviceProvider { get; set; }

        public BaseBooksState(EBooksContext db, IMapper mapper, IServiceProvider serviceProvider)
        {
            _db = db;
            _mapper = mapper;
            _serviceProvider = serviceProvider;
        }

        public virtual BooksRes Await(int id)
        {
            throw new Exception("Method not allowed");
        }

        public virtual BooksRes Approve(int id)
        {
            throw new Exception("Method not allowed");
        }

        public virtual BooksRes Reject(int id, string message)
        {
            throw new Exception("Method not allowed");
        }

        public virtual BooksRes Archive(int id)
        {
            throw new Exception("Method not allowed");
        }

        public BaseBooksState CheckState(string state)
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
                    return _serviceProvider.GetService<ArchiveBooksState>();
                default:
                    throw new Exception("State not recognized");
            }
        }
    }
}
