using ArchitectNow.Mongo.Db;

namespace ArchitectNow.Mongo.Services
{
	class DataContextService: IDataContextService<DataContext>
    {
	    public DataContext GetDataContext()
	    {
			return new DataContext();
	    }
    }
}
