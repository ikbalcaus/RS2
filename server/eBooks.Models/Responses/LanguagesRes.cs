namespace eBooks.Models.Responses
{
    public class LanguagesRes
    {
        public int LanguageId { get; set; }
        public string Name { get; set; }
        public UsersRes ModifiedBy { get; set; }
    }
}
