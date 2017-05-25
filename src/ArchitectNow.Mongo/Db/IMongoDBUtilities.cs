using System;
using MongoDB.Driver;

namespace ArchitectNow.Mongo.Db
{
    public interface IMongoDbUtilities : IDisposable 
    {
        IMongoDatabase Database { get; }
        string GetConnectionString();
        string GetDatabaseName();
    }
}
