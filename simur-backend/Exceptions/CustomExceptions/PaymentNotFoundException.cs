using System.Net;

namespace simur_backend.Exceptions.CustomExceptions
{
    public class PaymentCreationErrorException : Exception
    {
        public HttpStatusCode StatusCode { get; } = HttpStatusCode.InternalServerError;
        public PaymentCreationErrorException(string message) : base(message) { }
    }
}
