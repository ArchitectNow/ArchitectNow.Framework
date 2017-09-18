using System;
using ArchitectNow.Models.Contexts;
using ArchitectNow.Models.Options;

namespace ArchitectNow.Mongo.Models
{
    public class MongoDataContext:BaseDataContext<DataContextOptions>
    {
        public MongoDataContext(DataContextOptions options): base(options)
        {
            ResetContext();
            // ReSharper disable once VirtualMemberCallInConstructor
	        EnvironmentName = Options?.Environment;
        }

        public Guid CurrentOrganizationId { get; set; }
        public Guid CurrentUserId { get; set; }   
        public string CurrentCulture { get; set; }
        public string RootUrl { get; set; }
        public string EnvironmentName { get; set; }
        public string TimeZoneId { get; set; }

        public void ResetContext()
        {
            CurrentUserId = Guid.Empty;
            CurrentOrganizationId = Guid.Empty;
            CurrentCulture = "en-US";
            RootUrl = string.Empty;
            EnvironmentName = string.Empty;
            TimeZoneId = "Central Standard Time";
        }
    }
}
