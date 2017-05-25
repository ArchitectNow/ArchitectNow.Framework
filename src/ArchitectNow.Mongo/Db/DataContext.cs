using System;

namespace ArchitectNow.Mongo.Db
{
    public class DataContext
    {
        public DataContext(string environmentName = "")
        {
            ResetContext();
            EnvironmentName = environmentName;        
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
