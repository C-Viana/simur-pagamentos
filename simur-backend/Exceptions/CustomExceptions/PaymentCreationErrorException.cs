using System.Net;

namespace simur_backend.Exceptions.CustomExceptions
{
    public class PaymentNotFoundException : Exception
    {
        public HttpStatusCode StatusCode { get; } = HttpStatusCode.InternalServerError;
        public PaymentNotFoundException(string message) : base(message) { }
    }
}
