namespace ArchitectNow.Services.Contexts
{
	public interface IDataContextService<out TDataContext>
	{
		TDataContext GetDataContext();
	}
}