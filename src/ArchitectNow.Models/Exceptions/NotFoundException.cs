using System.Net;

namespace ArchitectNow.Models.Exceptions
{
    public class NotFoundException : ApiException
    {
        public NotFoundException(string type, object key):base(HttpStatusCode.NotFound, $"Could not find {key} for {type}.")
        {
                
        }
        
        public override object GetContent()
        {
            return null;
        }
    }
}