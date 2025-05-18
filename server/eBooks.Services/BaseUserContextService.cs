using eBooks.Database;
using System.Security.Claims;
using eBooks.Interfaces;
using eBooks.Models.Exceptions;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using eBooks.Database.Models;
using eBooks.Models;
using eBooks.Models.SearchObjects;

namespace eBooks.Services
{
    public abstract class BaseUserContextService<TEntity, TRequest, TResponse> : IBaseUserContextService<TRequest, TResponse>
        where TEntity : class, IUserBookEntity, new()
        where TRequest : class
        where TResponse : class
    {
        protected EBooksContext _db;
        protected IMapper _mapper;
        protected IHttpContextAccessor _httpContextAccessor;
        protected bool _shouldHaveBook;

        public BaseUserContextService(EBooksContext db, IMapper mapper, IHttpContextAccessor httpContextAccessor, bool shouldHaveBook)
        {
            _db = db;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _shouldHaveBook = shouldHaveBook;
        }

        protected int GetUserId() => int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : 0;
        protected async Task<bool> AccessRightExist(int bookId) => await _db.Set<AccessRight>().AnyAsync(x => x.UserId == GetUserId() && x.BookId == bookId);

        public virtual async Task<PagedResult<TResponse>> GetPaged()
        {
            var userId = GetUserId();
            var search = new BaseSearch();
            var entities = await _db.Set<TEntity>().Where(x => x.UserId == userId).ToListAsync();
            List<TResponse> result = new List<TResponse>();
            var query = _db.Set<TEntity>().AsQueryable();
            int count = await query.CountAsync();
            if (search?.Page.HasValue == true && search?.PageSize.HasValue == true)
                query = query.Skip(search.Page.Value * search.PageSize.Value).Take(search.PageSize.Value);
            var list = await query.ToListAsync();
            result = _mapper.Map(list, result);
            PagedResult<TResponse> pagedResult = new PagedResult<TResponse>
            {
                ResultList = result,
                Count = count
            };
            return pagedResult;
        }

        public virtual async Task<TResponse> GetByBookId(int bookId)
        {
            var userId = GetUserId();
            var entity = await _db.Set<TEntity>().Where(x => x.UserId == userId && x.BookId == bookId).FirstOrDefaultAsync();
            if (entity == null)
                throw new ExceptionNotFound();
            return _mapper.Map<TResponse>(entity);
        }

        public virtual async Task<TResponse> Post(int bookId, TRequest req)
        {
            if (!await _db.Set<Book>().AnyAsync(x => x.BookId == bookId))
                throw new ExceptionNotFound();
            var userId = GetUserId();
            if (await _db.Set<TEntity>().AnyAsync(x => x.UserId == userId && x.BookId == bookId))
                throw new ExceptionBadRequest("Already exist");
            var accessRightExist = await AccessRightExist(bookId);
            if (_shouldHaveBook && !accessRightExist)
                throw new ExceptionBadRequest("You have to possess this book");
            if (!_shouldHaveBook && accessRightExist)
                throw new ExceptionBadRequest("You already possess this book");
            var entity = new TEntity
            {
                UserId = userId,
                BookId = bookId
            };
            _mapper.Map(req, entity);
            _db.Set<TEntity>().Add(entity);
            await _db.SaveChangesAsync();
            return _mapper.Map<TResponse>(entity);
        }

        public virtual async Task<TResponse> Patch(int bookId)
        {
            var userId = GetUserId();
            var entity = await _db.Set<TEntity>().FirstOrDefaultAsync(x => x.UserId == userId && x.BookId == bookId);
            if (entity == null)
                throw new ExceptionNotFound();
            entity.ModifiedAt = DateTime.Now;
            await _db.SaveChangesAsync();
            return _mapper.Map<TResponse>(entity);
        }

        public virtual async Task<TResponse> Delete(int bookId)
        {
            var set = _db.Set<TEntity>();
            var userId = GetUserId();
            var entity = await set.FirstOrDefaultAsync(x => x.UserId == userId && x.BookId == bookId);
            if (entity == null)
                throw new ExceptionNotFound();
            set.Remove(entity);
            await _db.SaveChangesAsync();
            return null;
        }
    }
}
