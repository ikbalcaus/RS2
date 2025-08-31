using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Interfaces;
using eBooks.Models.Exceptions;
using eBooks.Models.Requests;
using eBooks.Models.Responses;
using eBooks.Models.Search;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace eBooks.Services
{
    public class ReviewsService : BaseUserContextService<Review, ReviewsSearch, ReviewsReq, ReviewsRes>, IReviewsService
    {
        public ReviewsService(EBooksContext db, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            : base(db, mapper, httpContextAccessor, true)
        {
        }

        public override IQueryable<Review> AddIncludes(IQueryable<Review> query, ReviewsSearch? search = null)
        {
            query = query.Include(x => x.User);
            if (search == null || search.IsBookIncluded == true)
                query = query.Include(x => x.Book);
            if (search == null || search.IsReportedByIncluded == true)
                query = query.Include(x => x.ReportedBy);
            return query;
        }

        public override IQueryable<Review> AddFilters(IQueryable<Review> query, ReviewsSearch search)
        {
            if (search.BookId != null)
                query = query.Where(x => x.BookId == search.BookId);
            if (!string.IsNullOrWhiteSpace(search.ReviewedBy))
                query = query.Where(x => x.User.UserName.ToLower().Contains(search.ReviewedBy.ToLower()));
            if (!string.IsNullOrWhiteSpace(search.BookTitle))
                query = query.Where(x => x.Book.Title.ToLower().Contains(search.BookTitle.ToLower()));
            if (search.IsReported == "Reported")
                query = query.Where(x => x.ReportedById != null);
            else if (search.IsReported == "Not reported")
                query = query.Where(x => x.ReportedById == null);
            if (search.OrderBy == "First added")
                query = query.OrderBy(x => x.ModifiedAt);
            else
                query = query.OrderByDescending(x => x.ModifiedAt);
            return query;
        }

        public override async Task<PagedResult<ReviewsRes>> GetPaged(ReviewsSearch search)
        {
            var result = new List<ReviewsRes>();
            var query = _db.Set<Review>().AsQueryable();
            query = AddIncludes(query, search);
            query = AddFilters(query, search);
            int count = await query.CountAsync();
            if (search?.Page.HasValue == true && search?.PageSize.HasValue == true && search.Page.Value > 0)
                query = query.Skip((search.Page.Value - 1) * search.PageSize.Value).Take(search.PageSize.Value);
            var list = await query.ToListAsync();
            result = _mapper.Map(list, result);
            PagedResult<ReviewsRes> pagedResult = new PagedResult<ReviewsRes>
            {
                ResultList = result,
                Count = count
            };
            return pagedResult;
        }

        public async Task<ReviewsRes> Put(int bookId, ReviewsReq req)
        {
            var set = _db.Set<Review>();
            var entity = await set.FindAsync(GetUserId(), bookId);
            if (entity == null)
                throw new ExceptionNotFound();
            if (!await AccessRightExist(bookId))
                throw new ExceptionBadRequest("You have to possess this book");
            _mapper.Map(req, entity);
            await _db.SaveChangesAsync();
            return _mapper.Map<ReviewsRes>(entity);
        }

        public async Task<ReviewsRes> Report(int userId, int bookId, string reason)
        {
            var set = _db.Set<Review>();
            var entity = await set.FindAsync(userId, bookId);
            if (entity == null)
                throw new ExceptionNotFound();
            entity.ReportedById = GetUserId();
            entity.ReportReason = reason;
            await _db.SaveChangesAsync();
            return _mapper.Map<ReviewsRes>(entity);
        }

        public async Task<ReviewsRes> AdminDelete(int userId, int bookId)
        {
            var set = _db.Set<Review>();
            var entity = await set.FindAsync(userId, bookId);
            if (entity == null) 
                throw new ExceptionNotFound();
            set.Remove(entity);
            await _db.SaveChangesAsync();
            return null;
        }
    }
}
