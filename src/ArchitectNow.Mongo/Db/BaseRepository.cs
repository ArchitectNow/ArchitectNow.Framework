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
    //Test notifications
    public abstract class BaseRepository<TModel, TDataContext> : IBaseRepository<TModel> where TModel : BaseDocument
        where TDataContext : MongoDataContext
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

        public virtual Task<List<TModel>> GetAllAsync(bool onlyActive = true)
        {
            Task<List<TModel>> results;
            if (onlyActive)
                results = GetCollection().Find(x => x.IsActive).ToListAsync();
            else
                results = GetCollection().Find(model => true).ToListAsync();

            return results;
        }

        public virtual Task<TModel> GetOneAsync(Guid id)
        {
            var result = GetCollection().Find(x => x.Id == id).FirstOrDefaultAsync();

            return result;
        }

        public virtual async Task<TModel> SaveAsync(TModel item)
        {
            if (item.Id != Guid.Empty)
                item.UpdatedDate = DateTime.UtcNow;

            if (item.OwnerUserId == null || item.OwnerUserId == Guid.Empty)
                if (CurrentContext != null && CurrentContext.CurrentUserId != Guid.Empty)
                    item.OwnerUserId = CurrentContext.CurrentUserId;

            var errors = await ValidateObject(item);

            if (errors.Any())
                throw new ValidationException("A validation error has occured saving item of type '" + item.GetType(),
                    errors);

            if (item.Id == Guid.Empty)
            {
                item.Id = Guid.NewGuid();
                await GetCollection().InsertOneAsync(item);
            }
            else
            {
                var filter = Builders<TModel>.Filter.Eq("_id", item.Id);
                await GetCollection().ReplaceOneAsync(filter, item, new UpdateOptions {IsUpsert = true});
            }

            Logger.LogInformation(EventIds.Update, "Entity Saved to {CollectionName}: \'{Id}\' - {@item}",
                CollectionName,
                item.Id, item);

            return item;
        }

        public virtual async Task<bool> DeleteAsync(Guid id)
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
        public virtual async Task ConfigureIndexes()
        {
            await CreateIndex("Id", Builders<TModel>.IndexKeys.Ascending(x => x.Id).Ascending(x => x.IsActive));
        }

        /// <summary>
        ///     Gets the collection.
        /// </summary>
        /// <returns></returns>
        protected IMongoCollection<TModel> GetCollection()
        {
            return _collection ?? (_collection = DbUtilities.Database.GetCollection<TModel>(CollectionName));
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

            await GetCollection().Indexes.CreateOneAsync(keys, options);
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
            return await GetCollection().CountAsync(filter);
        }

        protected async Task<List<TModel>> FindAsync(Expression<Func<TModel, bool>> filter)
        {
            return await GetCollection().Find(filter).ToListAsync();
        }
    }
}