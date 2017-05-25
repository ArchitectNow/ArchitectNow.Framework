using System.Linq;
using ArchitectNow.Mongo.Models;

namespace ArchitectNow.Mongo.Db
{
    public static class RepositoryExtensions
    {
        public static IQueryable<T> OnlyActive<T>(this IQueryable<T> source) where T : BaseDocument
        {
            return source.Where(x => x.IsActive);
        }
    }
}
