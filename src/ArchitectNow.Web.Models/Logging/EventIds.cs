using Microsoft.Extensions.Logging;

namespace ArchitectNow.Web.Models.Logging
{
    public class EventIds
    {
	    public static readonly EventId None = 0;
	    public static readonly EventId Create = 100;
	    public static readonly EventId Read = 200;
	    public static readonly EventId Update = 300;
	    public static readonly EventId Delete = 400;
	    public static readonly EventId Error = 500;
    }
}
