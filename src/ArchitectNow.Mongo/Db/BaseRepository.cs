using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ArchitectNow.Mongo.Models;
using ArchitectNow.Mongo.Services;
using ArchitectNow.Web.Models.Logging;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ArchitectNow.Mongo.Db
{
    //Test notifications
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : BaseDocument
    {
	    protected DataContext CurrentContext { get; }
	    protected ILogger<T> Logger { get; }
	    protected IMongoDbUtilities DbUtilities { get; }

	    /// <summary>
        /// Initializes a new instance of the <see cref="BaseRepository{T}"/> class.
        /// </summary>
        protected BaseRepository(ILogger<T> logger, IMongoDbUtilities mongoDbUtilities, IDataContextService<DataContext> dataContextService)
	    {
		    CurrentContext = dataContextService.GetDataContext();
		    Logger = logger;
		    DbUtilities = mongoDbUtilities;
	    }
		
        private string _regionName = "";

        /// <summary>
        /// Gets the name of the region.
        /// </summary>
        /// <value>
        /// The name of the region.
        /// </value>
        public string RegionName
        {
            get
            {
                if (string.IsNullOrEmpty(_regionName))
                {
                    _regionName = BuildCacheKeyPrefix().Replace(".", "");
					}
                return _regionName;
            }
        }

        private string _searchRegionName = "";
        /// <summary>
        /// Gets the name of the search region.
        /// </summary>
        /// <value>
        /// The name of the search region.
        /// </value>
        public string SearchRegionName
        {
            get
            {
                if (string.IsNullOrEmpty(_searchRegionName))
                {
                    _searchRegionName = RegionName + ".Searches".Replace(".", "");
                }

                return _searchRegionName;
            }
        }
		
        /// <summary>
        /// Determines whether [has valid user].
        /// </summary>
        /// <returns></returns>
        public bool HasValidUser()
        {
            return CurrentContext?.CurrentUserId != null && CurrentContext.CurrentUserId != Guid.Empty;
        }

        public abstract string CollectionName { get; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {

        }

        private IMongoCollection<T> _collection;
        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <returns></returns>
        public IMongoCollection<T> GetCollection()
        {
            if (_collection == null)
            {
                _collection = DbUtilities.Database.GetCollection<T>(CollectionName);
            }

            return _collection;
        }


        /// <summary>
        /// Builds the cache key prefix.
        /// </summary>
        /// <returns></returns>
        protected virtual string BuildCacheKeyPrefix()
        {
            var _base = GetType().ToString();
            
            if (CurrentContext?.CurrentOrganizationId != null && CurrentContext.CurrentOrganizationId != Guid.Empty)
            {
                _base += $".{CurrentContext.CurrentOrganizationId}";
            }
            return _base;
        }

        /// <summary>
        /// Builds the cache key.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        protected virtual string BuildCacheKey(string methodName, params string[] parameters)
        {
            var key = $"{BuildCacheKeyPrefix()}.{methodName}";

            if (parameters != null && parameters.Length > 0)
            {
                foreach (var param in parameters)
                {
                    key += ".-" + param;
                }
            }

            return key;
        }

        public virtual async Task<bool> DeleteAllAsync()
        {
            var filter = new BsonDocument();
            await GetCollection().DeleteManyAsync(filter);
			
            return true;
        }

        /// <summary>
        /// Deletes all.
        /// </summary>
        /// <returns></returns>
        public virtual bool DeleteAll()
        {

            var filter = new BsonDocument();
            GetCollection().DeleteMany(filter);

            return true;
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <param name="onlyActive">if set to <c>true</c> [only active].</param>
        /// <returns></returns>
        public virtual List<T> GetAll(bool onlyActive = true)
        {
	        List<T> results;
            
            if (onlyActive)
            {
                results = DataQuery.OnlyActive().ToList();
            }
            else
            {
                results = DataQuery.ToList();
            }

            return results;
        }

        public virtual async Task<List<T>> GetAllAsync(bool onlyActive = true)
        {
	        List<T> results;

            if (onlyActive)
            {
                results = await GetCollection().Find(x => x.IsActive).ToListAsync();
            }
            else
            {
                results = await GetCollection().Find(x => x.IsActive).ToListAsync();
            }

            return results;
        }

        /// <summary>
        /// Gets the one.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public virtual T GetOne(Guid id)
        {
	        var filter = Builders<T>.Filter.Eq("_id", id);
            
            var results = GetCollection().Find(filter).FirstOrDefault();

            return results;
        }

        public virtual async Task<T> GetOneAsync(Guid id)
        {
            var results = await GetCollection().Find(x => x.Id == id).FirstOrDefaultAsync();
			
            return results;
        }

        /// <summary>
        /// Saves the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        /// <exception cref="System.ComponentModel.DataAnnotations.ValidationException">
        /// A validation error has occured saving item of type ' + Item.GetType().ToString()
        /// or
        /// </exception>
        public virtual T Save(T item)
        {
            if (item.Id != Guid.Empty)
            {
                item.UpdatedDate = DateTime.UtcNow;
            }

            if (item.OwnerUserId == null || item.OwnerUserId == Guid.Empty)
            {
                if (CurrentContext != null && CurrentContext.CurrentUserId != Guid.Empty)
                {
                    item.OwnerUserId = CurrentContext.CurrentUserId;
                }
            }

            var validationSuccess = ValidateObject(item);

            if (validationSuccess)  // No errors
            {
                var existing = GetOne(item.Id);

                if (existing == null)
                {
                    GetCollection().InsertOne(item);
                }
                else
                {
                    var filter = Builders<T>.Filter.Eq("_id", item.Id);
                    GetCollection().ReplaceOne(filter, item);
                }
				
				Logger.LogInformation(EventIds.Update, "Entity Saved to {CollectionName}: \'{item.Id}\' - {@item}", CollectionName, item );
                
                return item;
            }
	        var first = item.ValidationErrors.FirstOrDefault();

	        if (first == null)
	        {
		        throw new ValidationException("A validation error has occured saving item of type '" + item.GetType());
	        }
	        var members = item.GetType() + " Message: " + first.ErrorMessage + ", Members: " + string.Join(",", first.MemberNames);

	        throw new ValidationException(members);
        }

        public virtual async Task<T> SaveAsync(T item)
        {
            if (item.Id != Guid.Empty)
            {
                item.UpdatedDate = DateTime.UtcNow;
            }

            if (item.OwnerUserId == null || item.OwnerUserId == Guid.Empty)
            {
                if (CurrentContext != null && CurrentContext.CurrentUserId != Guid.Empty)
                {
                    item.OwnerUserId = CurrentContext.CurrentUserId;
                }
            }

            var validationSuccess = ValidateObject(item);

            if (validationSuccess)  // No errors
            {
                var existing = GetOne(item.Id);

				if (existing == null)
                {
                    await GetCollection().InsertOneAsync(item);
                }
                else
                {
                    var filter = Builders<T>.Filter.Eq("_id", item.Id);
                    await GetCollection().ReplaceOneAsync(filter, item);
                }
				
	            Logger.LogInformation(EventIds.Update, "Entity Saved to {CollectionName}: \'{Id}\' - {@item}", CollectionName, item.Id, item);
				
                return item;
            }
	        var first = item.ValidationErrors.FirstOrDefault();

	        if (first == null)
	        {
		        throw new ValidationException("A validation error has occured saving item of type '" + item.GetType());
	        }
	        var members = item.GetType() + " Message: " + first.ErrorMessage + ", Members: " + string.Join(",", first.MemberNames);

	        throw new ValidationException(members);
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public virtual bool Delete(Guid id)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);
			
            GetCollection().DeleteOne(filter);

	        Logger.LogInformation(EventIds.Update, "Entity Deleted to {CollectionName}: \'{id}\'", CollectionName, id);
			
            return true;
        }

        public virtual async Task<bool> DeleteAsync(Guid id)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);

            await GetCollection().DeleteOneAsync(filter);

	        Logger.LogInformation(EventIds.Update, "Entity Deleted to {CollectionName}: \'{id}\'", CollectionName, id);

			return true;
        }

        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public virtual bool Delete(T item)
        {
            return Delete(item.Id);
        }

        public virtual async Task<bool> DeleteAsync(T item)
        {
            return await DeleteAsync(item.Id);
        }

        /// <summary>
        /// Configures the indexes.
        /// </summary>
        public virtual void ConfigureIndexes()
        {
            CreateIndex("Id", Builders<T>.IndexKeys.Ascending(x => x.Id).Ascending(x => x.IsActive));
        }

        public virtual void CreateIndex(string name, IndexKeysDefinition<T> keys)
        {
	        var options = new CreateIndexOptions<T>
	        {
		        Name = name
	        };

	        GetCollection().Indexes.CreateOne(keys, options);

        }

        /// <summary>
        /// Validates the object.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public virtual bool ValidateObject(T item)
        {
            var context = new ValidationContext(item);
            var results = new List<ValidationResult>();
            var success = Validator.TryValidateObject(item, context, results);

            if (!success)
            {
                item.ValidationErrors = results;
            }
            return success;
        }

        /// <summary>
        /// Gets the data query.
        /// </summary>
        /// <value>
        /// The data query.
        /// </value>
        public virtual IQueryable<T> DataQuery
        {
            get
            {
                if (_collection == null)
                {
                    _collection = DbUtilities.Database.GetCollection<T>(CollectionName);
                }

                return _collection.AsQueryable();

            }
        }

        //public virtual IAsync DataQueryAsync
        //{
        //    get
        //    {
        //        if (_collection == null)
        //        {
        //            _collection = DBUtilities.Database.GetCollection<T>(CollectionName);
        //        }

        //        return await _collection.FindAsync(new BsonDocument()).
        //    }


        //}

        public virtual async Task<long> CountAsync(Expression<Func<T, bool>> filter)
        {
            return await GetCollection().CountAsync(filter);
        }

        public async Task<List<T>> FindAsync(Expression<Func<T,bool>> filter)
        {
            return await GetCollection().Find(filter).ToListAsync();
        }
    }
}
