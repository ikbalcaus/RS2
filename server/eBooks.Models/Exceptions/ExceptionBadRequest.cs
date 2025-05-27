namespace eBooks.Models.Exceptions
{
    public class ExceptionBadRequest : Exception
    {
        public Dictionary<string, List<string>> Errors { get; set; }

        public ExceptionBadRequest(string message) : base(message)
        {
            Errors = new Dictionary<string, List<string>>
            {
                {
                    "error", new List<string> { message }
                }
            };
        }

        public ExceptionBadRequest(Dictionary<string, List<string>> errors)
        {
            Errors = errors;
        }
    }
}
