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
    public class BookGenresService : IBookGenresService
    {
        protected EBooksContext _db;
        protected IMapper _mapper;

        public BookGenresService(EBooksContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<List<BookGenresRes>> GetByBookId(int bookId)
        {
            var entities = await _db.Set<BookGenre>().Where(x => x.BookId == bookId).Include(x => x.Genre).OrderByDescending(x => x.ModifiedAt).ToListAsync();
            return _mapper.Map<List<BookGenresRes>>(entities);
        }

        public async Task<List<BookGenresRes>> Post(int bookId, BookGenresReq req)
        {
            var validGenreIds = await _db.Set<Genre>().Where(x => req.GenreIds.Contains(x.GenreId)).Select(x => x.GenreId).ToListAsync();
            if (validGenreIds.Count != req.GenreIds.Count)
                throw new ExceptionNotFound();
            var now = DateTime.UtcNow;
            var entities = req.GenreIds.Where(id => validGenreIds.Contains(id)).Select((genreId, index) => new BookGenre
            {
                BookId = bookId,
                GenreId = genreId,
                ModifiedAt = now.AddSeconds(-index)
            }).ToList();
            await _db.Set<BookGenre>().AddRangeAsync(entities);
            await _db.SaveChangesAsync();
            return _mapper.Map<List<BookGenresRes>>(entities);
        }

        public async Task<List<BookGenresRes>> Patch(int bookId, BookGenresReq req)
        {
            var validGenreIds = await _db.Set<Genre>().Where(x => req.GenreIds.Contains(x.GenreId)).Select(x => x.GenreId).ToListAsync();
            if (validGenreIds.Count != req.GenreIds.Count)
                throw new ExceptionNotFound();
            var now = DateTime.UtcNow;
            var selected = await _db.Set<BookGenre>().Where(x => x.BookId == bookId && validGenreIds.Contains(x.GenreId)).ToListAsync();
            if (selected.Count == 0)
                throw new ExceptionBadRequest("You must select at least one Genre");
            for (int i = 0; i < req.GenreIds.Count; i++)
            {
                var genreId = req.GenreIds[i];
                var entity = selected.FirstOrDefault(x => x.GenreId == genreId);
                if (entity != null)
                    entity.ModifiedAt = now.AddSeconds(-i);
            }
            await _db.SaveChangesAsync();
            return _mapper.Map<List<BookGenresRes>>(selected);
        }

        public async Task<BookGenresRes> Delete(int bookId, int genreId)
        {
            var set = _db.Set<BookGenre>();
            var entity = await set.FirstOrDefaultAsync(x => x.BookId == bookId && x.GenreId == genreId);
            if (entity == null)
                throw new ExceptionNotFound();
            set.Remove(entity);
            await _db.SaveChangesAsync();
            return null;
        }
    }
}
