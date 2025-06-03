namespace Application.Exceptions
{
    public class ExceptionBadRequest : Exception
    {
        public ExceptionBadRequest(string message) : base(message)
        {
        }

        public ExceptionBadRequest(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
