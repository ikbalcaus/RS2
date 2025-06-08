namespace eBooks.Models.Search
{
    public class QuestionsSearch : BaseSearch
    {
        public string? Question { get; set; }
        public string? AskedBy { get; set; }
        public string? Status { get; set; }
        public string? OrderBy { get; set; }
    }
}
