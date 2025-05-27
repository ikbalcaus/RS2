using Azure;
using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Interfaces;
using eBooks.Models.Exceptions;
using eBooks.Models.Requests;
using eBooks.Models.Responses;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace eBooks.Services
{
    public class ReviewsService : BaseUserContextService<Review, ReviewsPostReq, ReviewsRes>, IReviewService
    {
        public ReviewsService(EBooksContext db, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            : base(db, mapper, httpContextAccessor, true)
        {
        }

        public override async Task<ReviewsRes> Post(int bookId, ReviewsPostReq req)
        {
            if (req.Rating < 1 || req.Rating > 5)
                throw new ExceptionBadRequest("Rating must be between 1 and 5");
            return await base.Post(bookId, req);
        }

        public async Task<ReviewsRes> Put(int bookId, ReviewsPutReq req)
        {
            if (req.Rating < 1 || req.Rating > 5)
                throw new ExceptionBadRequest("Rating must be between 1 and 5");
            var set = _db.Set<Review>();
            if (!await _db.Set<Book>().AnyAsync(x => x.BookId == bookId))
                throw new ExceptionNotFound();
            var userId = GetUserId();
            if (!await AccessRightExist(bookId))
                throw new ExceptionBadRequest("You have to possess this book");
            var entity = await set.FindAsync(userId, bookId);
            _mapper.Map(req, entity);
            await _db.SaveChangesAsync();
            return _mapper.Map<ReviewsRes>(entity);
        }

        public async Task<ReviewsRes> DeleteByUserAndBookId(int userId, int bookId)
        {
            var set = _db.Set<Review>();
            var review = await set.FindAsync(userId, bookId);
            if (review == null) 
                throw new ExceptionNotFound();
            set.Remove(review);
            await _db.SaveChangesAsync();
            return null;
        }
    }
}
