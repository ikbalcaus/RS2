using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Interfaces;
using eBooks.Models.Exceptions;
using eBooks.Models.Requests;
using eBooks.Models.Responses;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace eBooks.Services
{
    public class BookAuthorsService : IBookAuthorsService
    {
        protected EBooksContext _db;
        protected IMapper _mapper;

        public BookAuthorsService(EBooksContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<List<BookAuthorsRes>> GetByBookId(int bookId)
        {
            var entities = await _db.Set<BookAuthor>().Where(x => x.BookId == bookId).Include(x => x.Author).OrderByDescending(x => x.ModifiedAt).ToListAsync();
            return _mapper.Map<List<BookAuthorsRes>>(entities);
        }

        public async Task<List<BookAuthorsRes>> Post(int bookId, BookAuthorsReq req)
        {
            var validAuthorIds = await _db.Set<Author>().Where(x => req.AuthorIds.Contains(x.AuthorId)).Select(x => x.AuthorId).ToListAsync();
            if (validAuthorIds.Count != req.AuthorIds.Count)
                throw new ExceptionNotFound();
            var now = DateTime.UtcNow;
            var entities = req.AuthorIds.Where(id => validAuthorIds.Contains(id)).Select((authorId, index) => new BookAuthor
            {
                BookId = bookId,
                AuthorId = authorId,
                ModifiedAt = now.AddSeconds(-index)
            }).ToList();
            await _db.Set<BookAuthor>().AddRangeAsync(entities);
            await _db.SaveChangesAsync();
            return _mapper.Map<List<BookAuthorsRes>>(entities);
        }

        public async Task<List<BookAuthorsRes>> Patch(int bookId, BookAuthorsReq req)
        {
            var validAuthorIds = await _db.Set<Author>().Where(x => req.AuthorIds.Contains(x.AuthorId)).Select(x => x.AuthorId).ToListAsync();
            if (validAuthorIds.Count != req.AuthorIds.Count)
                throw new ExceptionNotFound();
            var now = DateTime.UtcNow;
            var selected = await _db.Set<BookAuthor>().Where(x => x.BookId == bookId && validAuthorIds.Contains(x.AuthorId)).ToListAsync();
            if (selected.Count == 0)
                throw new ExceptionBadRequest("You must select at least one Author");
            for (int i = 0; i < req.AuthorIds.Count; i++)
            {
                var authorId = req.AuthorIds[i];
                var entity = selected.FirstOrDefault(x => x.AuthorId == authorId);
                if (entity != null)
                    entity.ModifiedAt = now.AddSeconds(-i);
            }
            await _db.SaveChangesAsync();
            return _mapper.Map<List<BookAuthorsRes>>(selected);
        }

        public async Task<BookAuthorsRes> Delete(int bookId, int authorId)
        {
            var set = _db.Set<BookAuthor>();
            var entity = await set.FirstOrDefaultAsync(x => x.BookId == bookId && x.AuthorId == authorId);
            if (entity == null)
                throw new ExceptionNotFound();
            set.Remove(entity);
            await _db.SaveChangesAsync();
            return null;
        }
    }
}
