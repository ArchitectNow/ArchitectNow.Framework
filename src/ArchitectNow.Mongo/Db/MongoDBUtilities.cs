using System;
using ArchitectNow.Mongo.Options;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace ArchitectNow.Mongo.Db
{
    public class MongoDbUtilities : IMongoDbUtilities
    {
	    private readonly MongoOptions _options;
	    private readonly string _dbName;
        private bool _isDisposed;
        private readonly MongoClient _client;

        public MongoDbUtilities(IOptions<MongoOptions> options )
        {
	        _options = options.Value;
	        var connectionString = GetConnectionString();

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("No DB connection found");
            }

            var pack = new ConventionPack{
                new EnumRepresentationConvention(BsonType.String),
				new CamelCaseElementNameConvention()
            };

            ConventionRegistry.Register("AN Conventions", pack, t => true);
            MongoDefaults.MaxConnectionIdleTime = TimeSpan.FromMinutes(1);
            
            _dbName = GetDatabaseName();
            
            if (string.IsNullOrEmpty(_dbName))
            {
                throw new Exception("No database name found");
            }

            _client = new MongoClient(connectionString);
        }

        public void CreateCollection(string collectionName)
        {

        }

        public string GetConnectionString()
        {
            return _options.ConnectionString;
        }

        public string GetDatabaseName()
        {
            return _options.DatabaseName;
        }

        public IMongoDatabase Database => _client.GetDatabase(_dbName);

	    public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    if (_client != null)
                    {
                        //used to disconnect here...
                    }
                }
            }

            _isDisposed = true;
        }

    

    }
}
