using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EFDM.Abstractions.Audit {

    public interface IDBContextAuditor {

        bool Enabled { get; set; }
        HashSet<string> GlobalIgnoredProperties { get; }
        HashSet<Type> IncludedTypes { get; }

        int SaveChanges(Func<int> baseSaveChanges);
        Func<IAuditEvent, IEventEntry, object, Task<bool>> GetMapperEventAction(Type type);
        Type GetEventType(Type type);
        Type GetPropertyType(Type type);
        void ExcludeProperty<T>(Expression<Func<T, object>> propertySelector);
        void Map<TSourceEntity, TAuditEventEntity, TAuditPropertyEntity>(
            Action<IAuditEvent, IEventEntry, TAuditEventEntity> eventAction);
        void SetEventCommonAction<T>(Action<IAuditEvent, IEventEntry, T> entityAction);
        void ExcludeTypeStateActions<TSourceEntity>(List<int> actions);
    }
}
