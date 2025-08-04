using System.Net;
using System.Runtime.Serialization;

namespace Dispatcher.Domain.Exceptions;

public class NotFoundHttpException: HttpException
{
    public NotFoundHttpException() { }

    public NotFoundHttpException(string message)
        : base(HttpStatusCode.NotFound, message) { }

    public NotFoundHttpException(string message, Exception inner)
        : base(message, inner) { }

    protected NotFoundHttpException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}