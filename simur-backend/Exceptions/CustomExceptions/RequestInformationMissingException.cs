using System.Net;

namespace simur_backend.Exceptions.CustomExceptions
{
    public class RequestInformationMissingException : Exception
    {
        public HttpStatusCode StatusCode { get; } = HttpStatusCode.BadRequest;
        public RequestInformationMissingException(string message) : base(message) { }
    }
}
