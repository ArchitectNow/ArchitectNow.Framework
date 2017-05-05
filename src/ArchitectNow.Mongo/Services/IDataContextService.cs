namespace ArchitectNow.Mongo.Services
{
	public interface IDataContextService<out TDataContext>
	{
		TDataContext GetDataContext();
	}
}