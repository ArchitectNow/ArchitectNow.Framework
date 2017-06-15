using System.Collections.Generic;
using System.Net;
using ArchitectNow.Models.Validation;
using Newtonsoft.Json;

namespace ArchitectNow.Models.Exceptions
{
    public class ValidationException : ApiException<IEnumerable<ValidationError>>
    {
	    public override string GetContent()
	    {
		    return JsonConvert.SerializeObject(Content);
	    }

	    public ValidationException(string message, IEnumerable<ValidationError> validationErrors  ) : base(HttpStatusCode.BadRequest, message, validationErrors ?? new List<ValidationError>())
        {
	        
        }
    }
}