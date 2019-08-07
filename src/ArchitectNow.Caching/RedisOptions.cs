namespace ArchitectNow.Caching
{
    class RedisOptions
    {
        public string ConnectionString { get; set; }
        public bool Enabled { get; set; }
        public int ExpirationInSeconds { get; set; } = CachingConstants.RedisDefaultExpirationInSeconds;
    }
}