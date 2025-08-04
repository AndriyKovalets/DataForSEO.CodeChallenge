using System.Net;
using System.Runtime.Serialization;

namespace Dispatcher.Domain.Exceptions;

public class HttpException: ApplicationException
{
    public virtual HttpStatusCode StatusCode { get; set; }
    
    public HttpException() { }

    public HttpException(HttpStatusCode statusCode, string message)
        : base(message)
    {
        this.StatusCode = statusCode;
    }

    public HttpException(string message, Exception inner)
        : base(message, inner) { }

    protected HttpException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}