using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using Newtonsoft.Json;

namespace ArchitectNow.Mongo.Models
{
    public abstract class BaseDocument
    {
	    protected BaseDocument()
        {
            ValidationErrors = new List<ValidationResult>();
        }

        [BsonId(IdGenerator = typeof(CombGuidGenerator))]
        public Guid Id { get; set; }

	    /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        /// <value>
        /// The created date.
        /// </value>
        public DateTimeOffset? CreatedDate { get; set; } = DateTime.UtcNow;

	    public DateTimeOffset? UpdatedDate { get; set; } = DateTime.UtcNow;
	    
        [JsonIgnore]
        [BsonIgnore]
        public List<ValidationResult> ValidationErrors { get; set; }

        [BsonExtraElements]
        public Dictionary<string, object> ExtraElements { get; set; }
    }
}
