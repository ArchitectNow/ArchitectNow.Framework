using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace ArchitectNow.Mongo.Db
{
    public abstract class MongoDbUtilities : IMongoDbUtilities
    {
        public string DatabaseName { get; }
        public string ConnectionString { get; }
        private bool _isDisposed;
        private readonly MongoClient _client;

        protected MongoDbUtilities(string connectionString, string databaseName)
        {
            DatabaseName = databaseName;
            ConnectionString = connectionString;
            
            DatabaseName = databaseName;
            ConnectionString = connectionString;
	        
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("No DB connection found");
            }
            
            if (string.IsNullOrEmpty(databaseName))
            {
                throw new Exception("No database name found");
            }
            
            var pack = new ConventionPack{
                new EnumRepresentationConvention(BsonType.String),
                new CamelCaseElementNameConvention()
            };

            ConventionRegistry.Register("AN Conventions", pack, t => true);
            MongoDefaults.MaxConnectionIdleTime = TimeSpan.FromMinutes(1);
            _client = new MongoClient(connectionString);            
        }

        public IMongoDatabase Database => _client.GetDatabase(DatabaseName);

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
