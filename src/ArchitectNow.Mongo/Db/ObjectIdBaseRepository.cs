using ArchitectNow.Mongo.Models;
using ArchitectNow.Services.Contexts;
using FluentValidation;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ArchitectNow.Mongo.Db
{
    public abstract class ObjectIdBaseRepository<TModel, TDataContext> : BaseRepository<TModel, TDataContext, ObjectId> where TModel : BaseDocument<ObjectId>
        where TDataContext : MongoDataContext
    {
        protected ObjectIdBaseRepository(ILogger<TModel> logger, IMongoDbUtilities mongoDbUtilities,
            IDataContextService<TDataContext> dataContextService,
            IValidator<TModel> validator = null) : base(logger, mongoDbUtilities, dataContextService, validator)
        {
        }

        protected override FilterDefinition<TModel> AreIdsEqual(ObjectId id)
        {
            var filter = new FilterDefinitionBuilder<TModel>().Where(x => x.Id == id);
            return filter;
        }

        protected override ObjectId CreateNewId() => ObjectId.GenerateNewId();
    }
}