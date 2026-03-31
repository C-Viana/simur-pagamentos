using System.Net;

namespace simur_backend.Exceptions.CustomExceptions
{
    public class InvalidEnvironmentSetupException : Exception
    {
        public HttpStatusCode StatusCode { get; } = HttpStatusCode.InternalServerError;
        public InvalidEnvironmentSetupException(string message) : base(message) { }
    }
}
