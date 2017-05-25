using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ArchitectNow.Mongo.Models;
using MongoDB.Driver;

namespace ArchitectNow.Mongo.Db
{
    public interface IBaseRepository<T> : IBaseRepository  
     where T : BaseDocument
    {
        bool Delete(Guid id);
        bool Delete(T item);
        bool DeleteAll();
        Task<bool> DeleteAllAsync();
        List<T> GetAll(bool onlyActive = true);
        T GetOne(Guid id);
        T Save(T item);
        bool ValidateObject(T item);
        IMongoCollection<T> GetCollection();
        IQueryable<T> DataQuery { get; }
        void CreateIndex(string name, IndexKeysDefinition<T> keys);
        Task<List<T>> GetAllAsync(bool onlyActive = true);
        Task<T> GetOneAsync(Guid id);
        Task<T> SaveAsync(T item);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> DeleteAsync(T item);
        Task<long> CountAsync(Expression<Func<T, bool>> filter);
        Task<List<T>> FindAsync(Expression<Func<T, bool>> filter);
    }

    public interface IBaseRepository : IDisposable
    {
        string CollectionName { get; }
        void ConfigureIndexes();
        bool HasValidUser();
        string RegionName { get; }
        string SearchRegionName { get; }

    }
}
