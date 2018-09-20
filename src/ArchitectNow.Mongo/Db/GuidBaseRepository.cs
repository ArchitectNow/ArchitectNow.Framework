using System;
using ArchitectNow.Mongo.Models;
using ArchitectNow.Services.Contexts;
using FluentValidation;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace ArchitectNow.Mongo.Db
{
    public abstract class GuidBaseRepository<TModel, TDataContext> : BaseRepository<TModel, TDataContext, Guid> where TModel : BaseDocument<Guid>
        where TDataContext : MongoDataContext
    {
        protected GuidBaseRepository(ILogger<TModel> logger, IMongoDbUtilities mongoDbUtilities,
            IDataContextService<TDataContext> dataContextService,
            IValidator<TModel> validator = null) : base(logger, mongoDbUtilities, dataContextService, validator)
        {
        }

        protected override FilterDefinition<TModel> AreIdsEqual(Guid id)
        {
            var filter = new FilterDefinitionBuilder<TModel>().Where(x => x.Id == id);
            return filter;
        }

        protected override Guid CreateNewId() => Guid.NewGuid();
    }
}