using EFDM.Core.Models.Domain;
using EFDM.Sample.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace EFDM.Sample.Core.Models.Domain
{
    public class UserAssignee : IdKeyEntityBase<int>
    {
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public int TypeId { get; set; }
        public virtual UserAssigneeType Type { get; set; }
        public int ObjectId { get; set; }
        public bool Active { get; set; }
        public DateTimeOffset Created { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public UserAssigneeData? Data { get; set; }

        public class UserAssigneeSearchRequest
        {
            public List<UserAssigneeSearchPair> Pairs { get; set; } = new();
            public UserAssigneeSearchRequest(List<UserAssigneeSearchPair> pairs)
            {
                Pairs = pairs ?? new List<UserAssigneeSearchPair>();
            }
        }
    }

    public class UserAssigneeData
    {
        public int? Assignment1Field1 { get; set; }
        public int? Assignment1Field2 { get; set; }
        public int? Assignment2Field1 { get; set; }
    }

    public class UserAssigneeSearchPair
    {
        public int TypeId { get; set; }
        public int ObjectId { get; set; }

        public UserAssigneeSearchPair(int typeId, int ObjectId)
        {
            this.TypeId = typeId;
            this.ObjectId = ObjectId;
        }
    }
}
