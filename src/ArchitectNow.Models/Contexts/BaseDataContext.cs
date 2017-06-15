
using ArchitectNow.Models.Options;

namespace ArchitectNow.Models.Contexts
{
	public abstract class BaseDataContext<TOptions> where TOptions: DataContextOptions, new()
	{
		protected virtual TOptions Options { get; }

		protected BaseDataContext(TOptions options)
	    {
		    Options = options;
	    }
    }
}
