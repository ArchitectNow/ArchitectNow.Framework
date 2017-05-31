using System.Net;

namespace ArchitectNow.Models.Exceptions
{
    public interface IApiException<TContent>
    {
        HttpStatusCode StatusCode { get; set; }
        TContent Content { get; set; }
    }
}