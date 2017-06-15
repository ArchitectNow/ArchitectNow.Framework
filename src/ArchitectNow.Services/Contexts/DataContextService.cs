using ArchitectNow.Models.Contexts;
using ArchitectNow.Models.Options;
using Microsoft.Extensions.Options;

namespace ArchitectNow.Services.Contexts
{
	public abstract class DataContextService<TDataContext, TOptions>: IDataContextService<TDataContext> where TDataContext: BaseDataContext<TOptions> where TOptions : DataContextOptions, new()
	{
		protected virtual TOptions Options { get; }
		public abstract TDataContext GetDataContext();

		protected DataContextService(IOptions<TOptions> options)
		{
			Options = options.Value;
		}
	}
}
