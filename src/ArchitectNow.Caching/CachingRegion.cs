namespace ArchitectNow.Caching
{
    public class CachingRegion : ICachingRegion
    {
        public const string DefaultRegion = "caching";
        
        public string Region { get; }

        public CachingRegion(string region = DefaultRegion)
        {
            Region = region;
        }
    }
}