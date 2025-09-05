namespace eBooks.Models.Responses
{
    public class OverviewRes
    {
        public int BooksCount { get; set; }
        public int ApprovedBooksCount { get; set; }
        public int AwaitedBooksCount { get; set; }
        public int DraftedCount { get; set; }
        public int HiddenCount { get; set; }
        public int RejectedCount { get; set; }
        public int UsersCount { get; set; }
        public int AuthorsCount { get; set; }
        public int GenresCount { get; set; }
        public int LanguagesCount { get; set; }
        public int QuestionsCount { get; set; }
        public int AnsweredQuestionsCount { get; set; }
        public int UnansweredQuestionsCount { get; set; }
        public int ReportsCount { get; set; }
        public int ReviewsCount { get; set; }
        public int PurchasesCount { get; set; }
    }
}
