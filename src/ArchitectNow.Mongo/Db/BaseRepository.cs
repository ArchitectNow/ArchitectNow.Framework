using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ArchitectNow.Models.Logging;
using ArchitectNow.Mongo.Models;
using ArchitectNow.Mongo.Services;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ArchitectNow.Mongo.Db
{
	//Test notifications
	public abstract class BaseRepository<T> : IBaseRepository<T> where T : BaseDocument
	{
		private readonly IValidator<T> _validator;
		protected ICacheService CacheService { get; }
		protected DataContext CurrentContext { get; }
		protected ILogger<T> Logger { get; }
		protected IMongoDbUtilities DbUtilities { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="BaseRepository{T}"/> class.
		/// </summary>
		protected BaseRepository(ILogger<T> logger, IMongoDbUtilities mongoDbUtilities, IDataContextService<DataContext> dataContextService, ICacheService cacheService, IValidator<T> validator)
		{
			_validator = validator;
			CacheService = cacheService;
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
					CacheService.CreateRegion(_regionName);
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
					CacheService.CreateRegion(_searchRegionName);
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
		protected IMongoCollection<T> GetCollection() => _collection ?? (_collection = DbUtilities.Database.GetCollection<T>(CollectionName));


		/// <summary>
		/// Builds the cache key prefix.
		/// </summary>
		/// <returns></returns>
		protected virtual string BuildCacheKeyPrefix()
		{
			return $"{GetType()}.{CurrentContext?.CurrentOrganizationId}";
		}

		/// <summary>
		/// Builds the cache key.
		/// </summary>
		/// <param name="methodName">Name of the method.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns></returns>
		protected virtual string BuildCacheKey(string methodName, params object[] parameters)
		{
			var key = $"{BuildCacheKeyPrefix()}.{methodName}";

			if (parameters != null && parameters.Length > 0)
			{
				key = parameters.Aggregate(key, (current, param) => current + (".-" + param));
			}

			return key;
		}

		public virtual async Task<bool> DeleteAllAsync()
		{
			CacheService.ClearRegion(RegionName);
			CacheService.ClearRegion(SearchRegionName);

			var filter = new BsonDocument();
			await GetCollection().DeleteManyAsync(filter);

			return true;
		}
		
		public virtual async Task<List<T>> GetAllAsync(bool onlyActive = true)
		{
            var cacheKey = BuildCacheKey(nameof(GetAllAsync), onlyActive);

            var results = CacheService.Get<List<T>>(cacheKey, SearchRegionName);
            if (results != null)
            {
                return results;
            }

			if (onlyActive)
			{
				results = await GetCollection().Find(x => x.IsActive).ToListAsync();
			}
			else
			{
				results = await GetCollection().Find(x => x.IsActive).ToListAsync();
			}

            CacheService.Add(cacheKey, results, SearchRegionName);
			return results;
		}
		
		public virtual async Task<T> GetOneAsync(Guid id)
		{
            var cacheKey = BuildCacheKey(nameof(GetOneAsync), id);

            var result = CacheService.Get<T>(cacheKey, RegionName);

            if (result != null)
            {
                return result;
            }

			result = await GetCollection().Find(x => x.Id == id).FirstOrDefaultAsync();

            CacheService.Add(cacheKey, result, RegionName);
			return result;
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

			var errors = await ValidateObject(item);

			if (errors.Any())
			{
				throw new ValidationException("A validation error has occured saving item of type '" + item.GetType(), errors);
			}

			var existing = await GetOneAsync(item.Id);

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

			//make sure we update the cache...
			var cacheKey = BuildCacheKey(nameof(GetOneAsync), item.Id);

			CacheService.Add(cacheKey, item, RegionName);

			CacheService.ClearRegion(SearchRegionName);
			return item;
		}
		
		public virtual async Task<bool> DeleteAsync(Guid id)
		{
			var filter = Builders<T>.Filter.Eq("_id", id);

			await GetCollection().DeleteOneAsync(filter);

			Logger.LogInformation(EventIds.Update, "Entity Deleted to {CollectionName}: \'{id}\'", CollectionName, id);

            var cacheKey = BuildCacheKey("GetOne", id);

            CacheService.Remove(cacheKey, RegionName);

            CacheService.ClearRegion(SearchRegionName);
			return true;
		}
		
		public virtual async Task<bool> DeleteAsync(T item)
		{
			var cacheKey = BuildCacheKey(nameof(GetOneAsync), item.Id);
			CacheService.Remove(cacheKey, RegionName);

			return await DeleteAsync(item.Id);
		}

		/// <summary>
		/// Configures the indexes.
		/// </summary>
		public virtual void ConfigureIndexes()
		{
			CreateIndex("Id", Builders<T>.IndexKeys.Ascending(x => x.Id).Ascending(x => x.IsActive));
		}

		protected virtual void CreateIndex(string name, IndexKeysDefinition<T> keys)
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
		protected virtual async Task<IList<ValidationFailure>> ValidateObject(T item)
		{
			var validationResult = await _validator.ValidateAsync(item);
			var validationResultErrors = validationResult.Errors;
			return validationResultErrors;
		}

		/// <summary>
		/// Gets the data query.
		/// </summary>
		/// <value>
		/// The data query.
		/// </value>
		protected virtual IQueryable<T> DataQuery
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

		protected virtual async Task<long> CountAsync(Expression<Func<T, bool>> filter)
		{
			return await GetCollection().CountAsync(filter);
		}

		protected async Task<List<T>> FindAsync(Expression<Func<T, bool>> filter)
		{
			return await GetCollection().Find(filter).ToListAsync();
		}
	}
}
