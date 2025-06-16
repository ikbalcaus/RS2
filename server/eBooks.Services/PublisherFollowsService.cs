using System.Security.Claims;
using Azure;
using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Interfaces;
using eBooks.Models.Exceptions;
using eBooks.Models.Responses;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace eBooks.Services
{
    public class PublisherFollowsService : IPublisherFollowsService
    {
        protected EBooksContext _db;
        protected IMapper _mapper;
        protected IHttpContextAccessor _httpContextAccessor;

        public PublisherFollowsService(EBooksContext db, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        protected int GetUserId() => int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : 0;

        public async Task<PagedResult<PublisherFollowsRes>> GetPaged()
        {
            var entities = await _db.Set<PublisherFollow>().Where(x => x.UserId == GetUserId()).Include(x => x.Publisher).ToListAsync();
            var result = _mapper.Map<List<PublisherFollowsRes>>(entities);
            return new PagedResult<PublisherFollowsRes>
            {
                Count = result.Count,
                ResultList = result
            };
        }

        public async Task<PublisherFollowsRes> GetByPublisherId(int publisherId)
        {
            var userId = GetUserId();
            var entity = await _db.Set<PublisherFollow>().FindAsync(userId, publisherId);
            if (entity == null)
                throw new ExceptionNotFound();
            return _mapper.Map<PublisherFollowsRes>(entity);
        }

        public async Task<PublisherFollowsRes> Post(int publisherId)
        {
            var set = _db.Set<PublisherFollow>();
            if (!await _db.Set<User>().AnyAsync(x => x.UserId == publisherId))
                throw new ExceptionNotFound();
            var userId = GetUserId();
            if (await set.AnyAsync(x => x.UserId == userId && x.PublisherId == publisherId))
                throw new ExceptionBadRequest("Already exist");
            var entity = new PublisherFollow
            {
                UserId = userId,
                PublisherId = publisherId,
            };
            set.Add(entity);
            await _db.SaveChangesAsync();
            return _mapper.Map<PublisherFollowsRes>(entity);
        }

        public async Task<PublisherFollowsRes> Delete(int publisherId)
        {
            var set = _db.Set<PublisherFollow>();
            var userId = GetUserId();
            var entity = await set.FirstOrDefaultAsync(x => x.UserId == userId && x.PublisherId == publisherId);
            if (entity == null)
                throw new ExceptionNotFound();
            set.Remove(entity);
            await _db.SaveChangesAsync();
            return null;
        }
    }
}
