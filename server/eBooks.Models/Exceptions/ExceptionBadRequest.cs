namespace eBooks.Models.Exceptions
{
    public class ExceptionBadRequest : Exception
    {
        public ExceptionBadRequest(string message) : base(message)
        {
        }
    }
}
