using Azure;
using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Interfaces;
using eBooks.Models.Exceptions;
using eBooks.Models.Responses;
using eBooks.Models.Search;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace eBooks.Services
{
    public class AccessRightsService : BaseUserContextService<AccessRight, BaseSearch, object, AccessRightsRes>, IAccessRightsService
    {
        public AccessRightsService(EBooksContext db, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            : base(db, mapper, httpContextAccessor, false)
        {
        }

        public override IQueryable<AccessRight> AddIncludes(IQueryable<AccessRight> query, BaseSearch? search = null)
        {
            query = query.Include(x => x.Book);
            return query;
        }

        public override async Task<PagedResult<AccessRightsRes>> GetPaged(BaseSearch search)
        {
            var userId = GetUserId();
            List<AccessRightsRes> result = new List<AccessRightsRes>();
            var query = _db.Set<AccessRight>().Where(x => x.UserId == userId && x.IsHidden == false).Include(x => x.Book).AsQueryable();
            int count = await query.CountAsync();
            if (search?.Page.HasValue == true && search?.PageSize.HasValue == true && search.Page.Value > 0)
                query = query.Skip((search.Page.Value - 1) * search.PageSize.Value).Take(search.PageSize.Value);
            var list = await query.OrderByDescending(x => x.IsFavorite).ThenByDescending(x => x.ModifiedAt).ToListAsync();
            result = _mapper.Map(list, result);
            PagedResult<AccessRightsRes> pagedResult = new PagedResult<AccessRightsRes>
            {
                ResultList = result,
                Count = count
            };
            return pagedResult;
        }

        public override async Task<AccessRightsRes> Post(int bookId, object req)
        {
            var errors = new Dictionary<string, List<string>>();
            var book = await _db.Set<Book>().FindAsync(bookId);
            if (book == null)
                throw new ExceptionNotFound();
            if (book.DeletionReason != null)
                throw new ExceptionBadRequest("This book is deleted");
            var userId = GetUserId();
            if (await _db.Set<AccessRight>().AnyAsync(x => x.UserId == userId && x.BookId == bookId))
                errors.AddError("Book", "You already possess this book");
            if (book.Price != 0)
                errors.AddError("Book", "This book is not free");
            if (userId == book.PublisherId)
                errors.AddError("Book", "You cannot buy your own book");
            if (book.StateMachine != "approve")
                errors.AddError("Book", "This book is not active right now");
            if (errors.Count > 0)
                throw new ExceptionBadRequest(errors);
            var entity = new AccessRight
            {
                UserId = userId,
                BookId = bookId
            };
            _db.Set<AccessRight>().Add(entity);
            await _db.SaveChangesAsync();
            return _mapper.Map<AccessRightsRes>(entity);
        }

        public override async Task<AccessRightsRes> Delete(int bookId)
        {
            var userId = GetUserId();
            var entity = await _db.Set<AccessRight>().FindAsync(userId, bookId);
            if (entity == null)
                throw new ExceptionNotFound();
            entity.IsHidden = !entity.IsHidden;
            await _db.SaveChangesAsync();
            return _mapper.Map<AccessRightsRes>(entity);
        }

        public async Task<AccessRightsRes> ToggleFavorite(int bookId)
        {
            var userId = GetUserId();
            var entity = await _db.Set<AccessRight>().FindAsync(userId, bookId);
            if (entity == null)
                throw new ExceptionNotFound();
            entity.IsFavorite = !entity.IsFavorite;
            entity.ModifiedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return _mapper.Map<AccessRightsRes>(entity);
        }

        public async Task<AccessRightsRes> SaveLastReadPage(int bookId, int page)
        {
            var userId = GetUserId();
            var entity = await _db.Set<AccessRight>().FindAsync(userId, bookId);
            if (entity == null)
                throw new ExceptionNotFound();
            entity.LastReadPage = page;
            entity.ModifiedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return _mapper.Map<AccessRightsRes>(entity);
        }
    }
}
