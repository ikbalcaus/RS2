using Azure.Core;
using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Interfaces;
using eBooks.Models.Books;
using eBooks.Services.BooksStateMachine;
using MapsterMapper;

namespace eBooks.Services
{
    public class BooksService : BaseService<Book, BooksSearch, BooksInsertReq, BooksUpdateReq, BooksRes>, IBooksService
    {
        public BaseBooksState _baseBooksState { get; set; }

        public BooksService(EBooksContext db, IMapper mapper, BaseBooksState baseProizvodiState) : base(db, mapper)
        {
            _baseBooksState = baseProizvodiState;
        }

        public override BooksRes Insert(BooksInsertReq req)
        {
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
            _mapper.Map(req, entity);
            entity.StateMachine = "draft";
            _db.SaveChanges();
            return _mapper.Map<BooksRes>(entity);
        }

        public BooksRes Await(int id)
        {
            var entity = GetById(id);
            var state = _baseBooksState.CheckState(entity.StateMachine);
            return state.Await(id);
        }

        public BooksRes Approve(int id)
        {
            var entity = GetById(id);
            var state = _baseBooksState.CheckState(entity.StateMachine);
            return state.Approve(id);
        }

        public BooksRes Reject(int id, string message)
        {
            var entity = GetById(id);
            var state = _baseBooksState.CheckState(entity.StateMachine);
            return state.Reject(id, message);
        }

        public BooksRes Archive(int id)
        {
            var entity = GetById(id);
            var state = _baseBooksState.CheckState(entity.StateMachine);
            return state.Archive(id);
        }
    }
}
