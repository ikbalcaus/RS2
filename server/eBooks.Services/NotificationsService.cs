using System.Security.Claims;
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
    public class NotificationsService : INotificationsService
    {
        protected EBooksContext _db;
        protected IMapper _mapper;
        protected IHttpContextAccessor _httpContextAccessor;

        public NotificationsService(EBooksContext db, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        protected int GetUserId() => int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : 0;

        public async Task<PagedResult<NotificationsRes>> GetByUserId(BaseSearch search)
        {
            var result = new List<NotificationsRes>();
            var query = _db.Set<Notification>().Where(x => x.UserId == GetUserId()).AsQueryable();
            int count = await query.CountAsync();
            if (search?.Page.HasValue == true && search?.PageSize.HasValue == true && search.Page.Value > 0)
                query = query.Skip((search.Page.Value - 1) * search.PageSize.Value).Take(search.PageSize.Value);
            var list = await query.OrderByDescending(x => x.ModifiedAt).ToListAsync();
            result = _mapper.Map(list, result);
            PagedResult<NotificationsRes> pagedResult = new PagedResult<NotificationsRes>
            {
                ResultList = result,
                Count = count
            };
            return pagedResult;
        }

        public async Task<NotificationsRes> MarkAsRead(int notificationId)
        {
            var entity = await _db.Set<Notification>().FindAsync(notificationId);
            if (entity == null)
                throw new ExceptionNotFound();
            if (entity.UserId != GetUserId())
                throw new ExceptionForbidden("Only owner can use this action");
            entity.IsRead = !entity.IsRead;
            await _db.SaveChangesAsync();
            return _mapper.Map<NotificationsRes>(entity);
        }

        public async Task<NotificationsRes> Delete(int notificationId)
        {
            var set = _db.Set<Notification>();
            var entity = await set.FindAsync(notificationId);
            if (entity == null)
                throw new ExceptionNotFound();
            if (entity.UserId != GetUserId())
                throw new ExceptionForbidden("Only owner can use this action");
            set.Remove(entity);
            await _db.SaveChangesAsync();
            return null;
        }
    }
}
