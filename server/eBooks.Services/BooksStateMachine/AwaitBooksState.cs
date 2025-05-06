using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Models.Books;
using MapsterMapper;

namespace eBooks.Services.BooksStateMachine
{
    public class AwaitBooksState : BaseBooksState
    {
        public AwaitBooksState(EBooksContext db, IMapper mapper, IServiceProvider serviceProvider)
            : base(db, mapper, serviceProvider)
        {
        }

        public async override Task<BooksRes> Update(int id, BooksUpdateReq req)
        {
            var set = _db.Set<Book>();
            var entity = await set.FindAsync(id);
            entity.StateMachine = "draft";
            await _db.SaveChangesAsync();
            if (req.Images != null && req.Images.Any()) Helpers.UploadImages(_db, _mapper, entity.BookId, req.Images);
            if (req.PdfFile != null) Helpers.UploadPdfFile(_db, _mapper, entity, req.PdfFile);
            return _mapper.Map<BooksRes>(entity);
        }

        public async override Task<BooksRes> Approve(int id)
        {
            var set = _db.Set<Book>();
            var entity = await set.FindAsync(id);
            entity.StateMachine = "approve";
            entity.RejectionReason = "";
            await _db.SaveChangesAsync();
            return _mapper.Map<BooksRes>(entity);
        }

        public async override Task<BooksRes> Reject(int id, string message)
        {
            var set = _db.Set<Book>();
            var entity = await set.FindAsync(id);
            entity.StateMachine = "reject";
            entity.RejectionReason = message;
            await _db.SaveChangesAsync();
            return _mapper.Map<BooksRes>(entity);
        }

        public async override Task<List<string>> AllowedActions(Book entity)
        {
            return new List<string>() { nameof(Update), nameof(Approve), nameof(Reject) };
        }
    }
}
