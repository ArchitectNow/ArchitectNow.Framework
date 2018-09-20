using System;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace ArchitectNow.Mongo.Models
{
    public abstract class GuidBaseDocument: BaseDocument<Guid>
    {
        [BsonId(IdGenerator = typeof(CombGuidGenerator))]
        public override Guid Id { get; set; }
    }
}