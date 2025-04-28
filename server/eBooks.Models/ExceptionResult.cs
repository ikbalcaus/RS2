namespace eBooks.Models
{
    public class ExceptionResult : Exception
    {
        public ExceptionResult(string message) : base(message)
        {
        }
    }
}
