using System;
using System.Net;

namespace ArchitectNow.Web.Models.Exceptions
{
    public abstract class ApiException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }
        public object Content { get; set; }
        public ApiException(string message, object content = null) : this(HttpStatusCode.BadRequest, message, null, content)
        {
        }

        public ApiException(HttpStatusCode statusCode, string message, object content = null) : this(statusCode, message, null, content)
        {
        }

        public ApiException(HttpStatusCode statusCode, string message, Exception innerException, object content = null) : base(message, innerException)
        {
            StatusCode = statusCode;
            Content = content;
        }

        public abstract object GetContent();
    }

    public class ApiException<TContent>: ApiException, IApiException<TContent>
    {
        public new TContent Content { get; set; }

        public ApiException(string message, TContent content = default(TContent)) : this(HttpStatusCode.BadRequest, message, null, content)
        {
        }

        public ApiException(HttpStatusCode statusCode, string message, TContent content = default(TContent)) : this(statusCode, message, null, content)
        {
        }

        public ApiException(HttpStatusCode statusCode, string message, Exception innerException, TContent content = default(TContent)) : base(message, innerException)
        {
            StatusCode = statusCode;
            Content = content;
        }

        public override object GetContent()
        {
            return Content;
        }
    }
}
