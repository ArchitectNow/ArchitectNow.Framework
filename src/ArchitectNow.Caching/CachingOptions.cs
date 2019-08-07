namespace ArchitectNow.Caching
{
    class CachingOptions
    {
        public bool Enabled { get; set; } = false;

        public int InMemoryExpirationInSeconds { get; set; } = CachingConstants.InMemoryDefaultExpirationInSeconds;
    }
}