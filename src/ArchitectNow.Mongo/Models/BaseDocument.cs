using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace ArchitectNow.Mongo.Models
{
    public abstract class BaseDocument<TId> where TId: IComparable<TId>, IEquatable<TId>
    {
        protected BaseDocument()
        {
            ValidationErrors = new List<ValidationResult>();
        }
        
        public abstract TId Id { get; set; }
        
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
