using EFDM.Abstractions.Models.Domain;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EFDM.Core.Models.Domain
{
    public abstract class EntityBase<TKey> : IdKeyEntityBase<TKey>, IEntityBase<TKey>
        where TKey : IComparable, IEquatable<TKey>
    {
        public DateTimeOffset Created { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset Modified { get; set; } = DateTimeOffset.Now;
        public int CreatedById { get; set; }
        public int ModifiedById { get; set; }
        IUser IAuditablePrincipalEntity.CreatedBy { get; set; }
        IUser IAuditablePrincipalEntity.ModifiedBy { get; set; }

        [NotMapped]
        [JsonIgnore]
        public bool PreserveLastModified { get; set; }
        [NotMapped]
        [JsonIgnore]
        public bool PreserveLastModifiedBy { get; set; }
        [NotMapped]
        [JsonIgnore]
        public bool PreserveLastModifiedFields
        {
            set
            {
                PreserveLastModified = value;
                PreserveLastModifiedBy = value;
            }
        }
    }
}
