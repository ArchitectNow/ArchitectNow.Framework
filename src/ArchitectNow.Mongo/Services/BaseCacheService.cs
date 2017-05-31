using ArchitectNow.Mongo.Options;
using Microsoft.Extensions.Options;

namespace ArchitectNow.Mongo.Services
{
	public class BaseCacheService<TOptions> : ICacheService where TOptions : CachingOptions, new()
	{
		private readonly TOptions _optionsValue;

		public BaseCacheService(IOptions<TOptions> options)
		{
			_optionsValue = options.Value;
			IsEnabled = _optionsValue.Enabled;
		}

		protected bool IsEnabled { get; }
		protected TOptions Options { get; }

		public virtual void Add(string key, object value, string regionName = "")
		{
		}

		public virtual T Get<T>(string key, string regionName = "")
		{
			return default(T);
		}

		public virtual object Get(string key, string regionName = "")
		{
			return null;
		}

		public virtual void Remove(string key, string regionName = "")
		{
		}

		public virtual void ClearCache()
		{
		}

		public virtual void CreateRegion(string regionName)
		{
		}

		public virtual void ClearRegion(string regionName)
		{
		}

		public virtual void ClearKeys(string prefix)
		{
		}
	}
}