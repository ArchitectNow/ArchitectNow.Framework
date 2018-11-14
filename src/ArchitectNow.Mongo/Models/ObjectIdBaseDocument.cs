using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace ArchitectNow.Mongo.Models
{
    public abstract class ObjectIdBaseDocument: BaseDocument<ObjectId>
    {
        [BsonId(IdGenerator = typeof(ObjectIdGenerator))]
        public override ObjectId Id { get; set; }
    }
}