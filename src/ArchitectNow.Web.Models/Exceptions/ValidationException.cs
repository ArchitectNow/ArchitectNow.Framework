using System.Collections.Generic;
using System.Net;
using ArchitectNow.Web.Models.Validation;

namespace ArchitectNow.Web.Models.Exceptions
{
    public class ValidationException : ApiException<IEnumerable<ValidationError>>
    {
        private IEnumerable<ValidationError> _validationErrors;

        public IEnumerable<ValidationError> Content
        {
            get => _validationErrors ??(_validationErrors = new List<ValidationError>() );
	        set => _validationErrors = value;
        }
        
        public ValidationException(string message, IEnumerable<ValidationError> validationErrors  ) : base(HttpStatusCode.BadRequest, message, validationErrors)
        {
            
        }
    }
}