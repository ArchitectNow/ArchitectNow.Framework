using Newtonsoft.Json;

namespace ArchitectNow.Models.ViewModels
{
	public class ApiError
	{
		public string Error { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string StackTrace { get; set; }
	}
}
