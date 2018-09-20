using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArchitectNow.Mongo.Models;

namespace ArchitectNow.Mongo.Db
{
    public interface IBaseRepository<T, TId> : IBaseRepository  
     where T : BaseDocument<TId>
     where TId: IComparable<TId>, IEquatable<TId>
    {
        Task<bool> DeleteAllAsync();
        Task<List<T>> GetAllAsync();
        Task<T> GetOneAsync(TId id);
        Task<T> SaveAsync(T item);
        Task<bool> DeleteAsync(TId id);
        Task<bool> DeleteAsync(T item);
    }

    public interface IBaseRepository : IDisposable
    {
        string CollectionName { get; }
        Task ConfigureIndexes();
        bool HasValidUser();
    }
}
