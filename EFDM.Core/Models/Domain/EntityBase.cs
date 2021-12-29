﻿using EFDM.Abstractions.Models.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EFDM.Core.Models.Domain {

    public abstract class EntityBase<TKey> : IdKeyEntityBase<TKey>, IEntityBase<TKey>        
        where TKey : IComparable, IEquatable<TKey> {

        public DateTimeOffset Created { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset Modified { get; set; } = DateTimeOffset.Now;
        public int CreatedById { get; set; }
        public int ModifiedById { get; set; }
        public IUser CreatedBy { get; set; }
        public IUser ModifiedBy { get; set; }

        [NotMapped]
        [JsonIgnore]
        public bool PreserveLastModifiedInfo { set; get; }        
    }
}