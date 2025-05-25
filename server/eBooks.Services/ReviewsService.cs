using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Interfaces;
using eBooks.Models.Exceptions;
using eBooks.Models.Requests;
using eBooks.Models.Responses;
using MapsterMapper;
using Microsoft.AspNetCore.Http;

namespace eBooks.Services
{
    public class ReviewsService : BaseUserContextService<Review, ReviewsReq, ReviewsRes>, IReviewService
    {
        public ReviewsService(EBooksContext db, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            : base(db, mapper, httpContextAccessor, true)
        {
        }

        public override async Task<ReviewsRes> Post(int bookId, ReviewsReq req)
        {
            if (req.Rating < 1 || req.Rating > 5)
                throw new ExceptionBadRequest("Rating must be between 1 and 5");
            return await base.Post(bookId, req);
        }
    }
}
