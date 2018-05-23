using System;
using MongoDB.Driver;

namespace ArchitectNow.Mongo.Db
{
    public interface IMongoDbUtilities : IDisposable 
    {
        IMongoDatabase Database { get; }
        string DatabaseName { get; }
        string ConnectionString { get; }
    }
}
