using ArchitectNow.Models.Options;
using ArchitectNow.Mongo.Models;
using ArchitectNow.Services.Contexts;
using Microsoft.Extensions.Options;

namespace ArchitectNow.Mongo.Services
{
	class MongoDataContextService : DataContextService<MongoDataContext, DataContextOptions>
	{
		public MongoDataContextService(IOptions<DataContextOptions> options) : base(options)
		{

		}

		public override MongoDataContext GetDataContext()
		{
			return new MongoDataContext(Options);
		}
	}
}
