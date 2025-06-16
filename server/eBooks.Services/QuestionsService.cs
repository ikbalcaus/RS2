using System.Security.Claims;
using EasyNetQ;
using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Interfaces;
using eBooks.Models.Exceptions;
using eBooks.Models.Messages;
using eBooks.Models.Requests;
using eBooks.Models.Responses;
using eBooks.Models.Search;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace eBooks.Services
{
    public class QuestionsService : BaseReadOnlyService<Question, QuestionsSearch, QuestionsRes>, IQuestionsService
    {
        protected IHttpContextAccessor _httpContextAccessor;
        protected IBus _bus;

        public QuestionsService(EBooksContext db, IMapper mapper, IHttpContextAccessor httpContextAccessor, IBus bus)
            : base(db, mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _bus = bus;
        }

        protected int GetUserId() => int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : 0;

        public override IQueryable<Question> AddFilters(IQueryable<Question> query, QuestionsSearch search)
        {
            if (!string.IsNullOrWhiteSpace(search.Question))
                query = query.Where(x => x.Question1.ToLower().Contains(search.Question.ToLower()));
            if (!string.IsNullOrWhiteSpace(search.AskedBy))
                query = query.Where(x => x.User.UserName.ToLower().Contains(search.AskedBy.ToLower()));
            if (search.Status == "Only answered")
                query = query.Where(x => x.AnsweredById != null);
            if (search.Status == "Only not answered")
                query = query.Where(x => x.AnsweredById == null);
            if (search.OrderBy == "First added")
                query = query.OrderBy(x => x.ModifiedAt);
            else
                query = query.OrderByDescending(x => x.ModifiedAt);
            return query;
        }

        public async Task<QuestionsRes> Post(QuestionsReq req)
        {
            var entity = new Question
            {
                UserId = GetUserId(),
                Question1 = req.Message
            };
            _db.Add(entity);
            await _db.SaveChangesAsync();
            return _mapper.Map<QuestionsRes>(entity);
        }

        public async Task<QuestionsRes> Patch(int id, QuestionsReq req)
        {
            var entity = await _db.Set<Question>().Include(x => x.User).FirstOrDefaultAsync(x => x.QuestionId == id);
            if (entity == null)
                throw new ExceptionNotFound();
            entity.AnsweredById = GetUserId();
            entity.Answer = req.Message;
            await _db.SaveChangesAsync();
            var result = _mapper.Map<QuestionsRes>(entity);
            _bus.PubSub.Publish(new QuestionAnswered { Question = result });
            return result;
        }
    }
}
