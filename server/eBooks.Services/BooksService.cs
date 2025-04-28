using Azure.Core;
using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Interfaces;
using eBooks.Models.Books;
using eBooks.Models.User;
using eBooks.Services.BooksStateMachine;
using MapsterMapper;
using Microsoft.Extensions.Logging;

namespace eBooks.Services
{
    public class BooksService : BaseService<Book, BooksSearch, BooksCreateReq, BooksUpdateReq, BooksRes>, IBooksService
    {
        protected ILogger<BooksService> _logger;
        protected BaseBooksState _baseBooksState { get; set; }

        public BooksService(EBooksContext db, IMapper mapper, ILogger<BooksService> logger, BaseBooksState baseProizvodiState) : base(db, mapper)
        {
            _logger = logger;
            _baseBooksState = baseProizvodiState;
        }

        public override BooksRes Create(BooksCreateReq req)
        {
            _logger.LogInformation($"Book with title {req.Title} created.");
            var set = _db.Set<Book>();
            var entity = _mapper.Map<Book>(req);
            entity.StateMachine = "draft";
            set.Add(entity);
            _db.SaveChanges();
            return _mapper.Map<BooksRes>(entity);
        }

        public override BooksRes Update(int id, BooksUpdateReq req)
        {
            var set = _db.Set<Book>();
            var entity = set.Find(id);
            _logger.LogInformation($"Book with title {entity.Title} updated.");
            _mapper.Map(req, entity);
            entity.StateMachine = "draft";
            entity.RejectionReason = null;
            _db.SaveChanges();
            return _mapper.Map<BooksRes>(entity);
        }

        public override void Delete(int id)
        {
            var set = _db.Set<Book>();
            var entity = set.Find(id);
            _logger.LogInformation($"Book with title {entity.Title} deleted.");
            set.Remove(entity);
            _db.SaveChanges();
        }

        public BooksRes Await(int id)
        {
            var entity = GetById(id);
            var state = _baseBooksState.CheckState(entity.StateMachine);
            _logger.LogInformation($"Book with title {entity.Title} awaited.");
            return state.Await(id);
        }

        public BooksRes Approve(int id)
        {
            var entity = GetById(id);
            var state = _baseBooksState.CheckState(entity.StateMachine);
            _logger.LogInformation($"Book with title {entity.Title} approved.");
            return state.Approve(id);
        }

        public BooksRes Reject(int id, string message)
        {
            var entity = GetById(id);
            var state = _baseBooksState.CheckState(entity.StateMachine);
            _logger.LogInformation($"Book with title {entity.Title} rejected with message \"{message}\".");
            return state.Reject(id, message);
        }

        public BooksRes Archive(int id)
        {
            var entity = GetById(id);
            var state = _baseBooksState.CheckState(entity.StateMachine);
            _logger.LogInformation($"Book with title {entity.Title} archived/unarchived.");
            return state.Archive(id);
        }

        public List<string> AllowedActions(int id)
        {
            var entity = _db.Books.Find(id);
            var state = _baseBooksState.CheckState(entity.StateMachine);
            return state.AllowedActions(entity);
        }
    }
}
