using EFDM.Abstractions.Models.Domain;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace EFDM.Core.Models.Domain
{
    public abstract class IdKeyEntityBase<TKey> : IIdKeyEntity<TKey>
        where TKey : IComparable, IEquatable<TKey>
    {
        public TKey Id { get; set; }

        #region for DBContextAuditor -> GetPropertyChanges purposes

        [JsonIgnore]
        [XmlIgnore]
        object IEntity.Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        #endregion for DBContextAuditor -> GetPropertyChanges purposes

        [NotMapped]
        [JsonIgnore]
        public bool IsNew
        {
            get { return Id.Equals(default); }
        }
    }
}
