using System.Net;

namespace simur_backend.Exceptions.CustomExceptions
{
    public class PaymentCreationException : Exception
    {
        public HttpStatusCode StatusCode { get; } = HttpStatusCode.InternalServerError;
        public PaymentCreationException(string message) : base(message) { }
    }
}
