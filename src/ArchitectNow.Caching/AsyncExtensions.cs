using System;
using System.Threading.Tasks;

namespace ArchitectNow.Caching
{
    static class AsyncExtensions
    {
        public static Task<T> AsResult<T>(this T result)
        {
            return Task.FromResult(result);
        }
        
        public static Task Invoke(this Action action)
        {
            return Task.Run(action);
        }
    }
}