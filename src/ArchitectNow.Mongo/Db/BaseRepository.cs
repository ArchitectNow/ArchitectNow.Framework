using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ArchitectNow.Models.Logging;
using ArchitectNow.Mongo.Models;
using ArchitectNow.Services.Contexts;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ArchitectNow.Mongo.Db
{
    public abstract class BaseRepository<TModel, TDataContext, TId> : IBaseRepository<TModel, TId> where TModel : BaseDocument<TId>
        where TDataContext : MongoDataContext
        where TId : IComparable<TId>
    {
        private readonly IValidator<TModel> _validator;

        private IMongoCollection<TModel> _collection;

        protected BaseRepository(ILogger<TModel> logger, IMongoDbUtilities mongoDbUtilities,
            IDataContextService<TDataContext> dataContextService,
            IValidator<TModel> validator = null)
        {
            _validator = validator ?? new InlineValidator<TModel>();

            CurrentContext = dataContextService.GetDataContext();
            Logger = logger;
            DbUtilities = mongoDbUtilities;
        }

        protected TDataContext CurrentContext { get; }
        protected ILogger<TModel> Logger { get; }
        protected IMongoDbUtilities DbUtilities { get; }

        /// <summary>
        ///     Gets the data query.
        /// </summary>
        /// <value>
        ///     The data query.
        /// </value>
        protected virtual IQueryable<TModel> DataQuery
        {
            get
            {
                if (_collection == null)
                    _collection = DbUtilities.Database.GetCollection<TModel>(CollectionName);

                return _collection.AsQueryable();
            }
        }

        /// <summary>
        ///     Determines whether [has valid user].
        /// </summary>
        /// <returns></returns>
        public bool HasValidUser()
        {
            return CurrentContext?.CurrentUserId != null && CurrentContext.CurrentUserId != Guid.Empty;
        }

        public abstract string CollectionName { get; }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
        }

        public virtual async Task<bool> DeleteAllAsync()
        {
            var filter = new BsonDocument();
            await GetCollection().DeleteManyAsync(filter);

            return true;
        }

        public virtual Task<List<TModel>> GetAllAsync()
        {
            var results = GetCollection().Find(model => true).ToListAsync();

            return results;
        }

        protected abstract FilterDefinition<TModel> AreIdsEqual(TId id);
        protected abstract TId CreateNewId();

        public virtual Task<TModel> GetOneAsync(TId id)
        {
            var filter = AreIdsEqual(id);

            var result = GetCollection().Find(filter).FirstOrDefaultAsync();

            return result;
        }

        public virtual async Task<TModel> SaveAsync(TModel item)
        {
            if (!Equals(item.Id, default(TId)))
                item.UpdatedDate = DateTime.UtcNow;

            var errors = await ValidateObject(item);

            if (errors.Any())
                throw new ValidationException("A validation error has occured saving item of type '" + item.GetType(),
                    errors);

            if (Equals(item.Id, default(TId)))
            {
                item.Id = CreateNewId();
                await GetCollection().InsertOneAsync(item);
            }
            else
            {
                var filter = Builders<TModel>.Filter.Eq("_id", item.Id);
                await GetCollection().ReplaceOneAsync(filter, item, new ReplaceOptions() {IsUpsert = true});
            }

            Logger.LogInformation(EventIds.Update, "Entity Saved to {CollectionName}: \'{Id}\' - {@item}",
                CollectionName,
                item.Id, item);

            return item;
        }

        public virtual async Task<bool> DeleteAsync(TId id)
        {
            var filter = Builders<TModel>.Filter.Eq("_id", id);

            await GetCollection().DeleteOneAsync(filter);

            Logger.LogInformation(EventIds.Update, "Entity Deleted to {CollectionName}: \'{id}\'", CollectionName, id);

            return true;
        }

        public virtual Task<bool> DeleteAsync(TModel item)
        {
            return DeleteAsync(item.Id);
        }

        /// <summary>
        ///     Configures the indexes.
        /// </summary>
        public virtual Task ConfigureIndexes()
        {
            return Task.Run(() => { });
        }

        /// <summary>
        ///     Gets the collection.
        /// </summary>
        /// <returns></returns>
        protected IMongoCollection<TModel> GetCollection()
        {
            return _collection ??= DbUtilities.Database.GetCollection<TModel>(CollectionName);
        }


        /// <summary>
        ///     Builds the cache key prefix.
        /// </summary>
        /// <returns></returns>
        protected virtual string BuildCacheKeyPrefix()
        {
            return $"{GetType()}.{CurrentContext?.CurrentOrganizationId}";
        }

        /// <summary>
        ///     Builds the cache key.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        protected virtual string BuildCacheKey(string methodName, params object[] parameters)
        {
            var key = $"{BuildCacheKeyPrefix()}.{methodName}";

            if (parameters != null && parameters.Length > 0)
                key = parameters.Aggregate(key, (current, param) => current + (".-" + param));

            return key;
        }

        protected virtual async Task CreateIndex(string name, IndexKeysDefinition<TModel> keys)
        {
            var options = new CreateIndexOptions<TModel>
            {
                Name = name
            };

            var createIndexModel = new CreateIndexModel<TModel>(keys, options);

            await GetCollection().Indexes.CreateOneAsync(createIndexModel);
        }

        /// <summary>
        ///     Validates the object.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        protected virtual async Task<IList<ValidationFailure>> ValidateObject(TModel item)
        {
            var validationResult = await _validator.ValidateAsync(item);
            var validationResultErrors = validationResult.Errors;
            return validationResultErrors;
        }

        protected virtual async Task<long> CountAsync(Expression<Func<TModel, bool>> filter)
        {
            return await GetCollection().CountDocumentsAsync(filter);
        }

        protected async Task<List<TModel>> FindAsync(Expression<Func<TModel, bool>> filter)
        {
            return await GetCollection().Find(filter).ToListAsync();
        }
    }
}