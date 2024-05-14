﻿using EFDM.Abstractions.Models.Domain;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EFDM.Core.Models.Domain
{
    public abstract class IdKeyEntityBase<TKey> : IIdKeyEntity<TKey>
        where TKey : IComparable, IEquatable<TKey>
    {
        public TKey Id { get; set; }

        [NotMapped]
        [JsonIgnore]
        public bool IsNew
        {
            get { return Id.Equals(default); }
        }
    }
}
