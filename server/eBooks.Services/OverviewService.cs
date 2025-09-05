using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Interfaces;
using eBooks.Models.Responses;
using Microsoft.EntityFrameworkCore;

namespace eBooks.Services
{
    public class OverviewService : IOverviewService
    {
        protected EBooksContext _db;

        public OverviewService(EBooksContext db)
        {
            _db = db;
        }

        public async Task<OverviewRes> GetAllCount()
        {
            var overview = new OverviewRes();
            overview.BooksCount = await _db.Set<Book>().CountAsync();
            overview.ApprovedBooksCount = await _db.Set<Book>().CountAsync(x => x.StateMachine == "approve");
            overview.AwaitedBooksCount = await _db.Set<Book>().CountAsync(x => x.StateMachine == "await");
            overview.DraftedCount = await _db.Set<Book>().CountAsync(x => x.StateMachine == "draft");
            overview.HiddenCount = await _db.Set<Book>().CountAsync(x => x.StateMachine == "hide");
            overview.RejectedCount = await _db.Set<Book>().CountAsync(x => x.StateMachine == "reject");
            overview.UsersCount = await _db.Set<User>().CountAsync();
            overview.AuthorsCount = await _db.Set<Author>().CountAsync();
            overview.GenresCount = await _db.Set<Genre>().CountAsync();
            overview.LanguagesCount = await _db.Set<Language>().CountAsync();
            overview.QuestionsCount = await _db.Set<Question>().CountAsync();
            overview.AnsweredQuestionsCount = await _db.Set<Question>().CountAsync(x => x.AnsweredById != null);
            overview.UnansweredQuestionsCount = await _db.Set<Question>().CountAsync(x => x.AnsweredById == null);
            overview.ReportsCount = await _db.Set<Report>().CountAsync();
            overview.ReviewsCount = await _db.Set<Review>().CountAsync();
            overview.PurchasesCount = await _db.Set<Purchase>().CountAsync();
            return overview;
        }
    }
}
