using EFDM.Abstractions.DataQueries;
using EFDM.Core.DataQueries;
using EFDM.Core.Extensions;
using EFDM.Sample.Core.Models.Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EFDM.Sample.Core.DataQueries.Models;

public class UserAssigneeQuery : IdKeyDataQueryBase<UserAssignee, int>
{
    public int? UserAssigneeId { get; set; }
    public int? UserId { get; set; }
    public int[] UserIds { get; set; }
    public int? TypeId { get; set; }
    public int[] TypeIds { get; set; }
    public long? ObjectId { get; set; }
    public long?[] ObjectIds { get; set; }
    public bool? Active { get; set; }
    public bool? FitNowRange { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    // Фильтры по парам TypeId  ObjectId
    public List<UserAssigneeSearchPair> TypeObjectPairs { get; set; } = new();

    // Фильтры по полям в Data
    public int? Assignment1Field1 { get; set; }
    public int? Assignment1Field2 { get; set; }
    public int? Assignment2Field1 { get; set; }
    public int[]? Assignment1Field1Array { get; set; }

    string FormatAsJsonValue(DateTimeOffset dt)
    {
        return JsonConvert.SerializeObject(dt); // Uses ISO 8601: "2025-01-01T00:00:00+00:00"
    }

    public override IQueryFilter<UserAssignee> ToFilter()
    {
        var and = new QueryFilter<UserAssignee>();

        if (UserId.HasValue)
            and.Add(x => x.UserId == UserId);

        if (UserIds?.Any() == true)
        {
            var nullableUserIds = UserIds.Select(id => (int?)id).ToArray();
            and.Add(x => nullableUserIds.Contains(x.UserId));
        }

        if (TypeObjectPairs?.Any() == true)
        {
            Expression<Func<UserAssignee, bool>> orExpr = x => false;

            foreach (var p in TypeObjectPairs)
            {
                var pair = p; // важно
                orExpr = orExpr.Or(x =>
                    x.TypeId == pair.TypeId &&
                    x.ObjectId == pair.ObjectId
                );
            }

            and.Add(orExpr);
        }

        if (TypeId.HasValue)
            and.Add(x => x.TypeId == TypeId);

        if (TypeIds?.Any() == true)
            and.Add(x => TypeIds.Contains(x.TypeId));

        if (ObjectId.HasValue)
            and.Add(x => x.ObjectId == ObjectId);

        if (ObjectIds?.Any() == true)
            and.Add(x => ObjectIds.Contains(x.ObjectId));

        if (Active.HasValue)
            and.Add(x => x.Active == Active);

        if (StartDate.HasValue && EndDate.HasValue)
            and.Add(x => x.StartDate >= StartDate && x.EndDate <= EndDate);

        if (FitNowRange.HasValue)
        {
            if (FitNowRange.Value == true)
                and.Add(x => x.StartDate <= DateTimeOffset.Now && x.EndDate >= DateTimeOffset.Now);
        }

        if (Assignment1Field1.HasValue)
            and.Add(x => x.Data != null && x.Data.Assignment1Field1 == Assignment1Field1);

        if (Assignment1Field2.HasValue)
            and.Add(x => x.Data != null && x.Data.Assignment1Field2 == Assignment1Field2);

        if (Assignment2Field1.HasValue)
            and.Add(x => x.Data != null && x.Data.Assignment2Field1 == Assignment2Field1);

        if (Assignment1Field1Array?.Any() == true)
            and.Add(x =>
                x.Data != null && x.Data.Assignment1Field1.HasValue && Assignment1Field1Array.Contains(x.Data.Assignment1Field1.Value));

        return base.ToFilter().Add(and);
    }
}
