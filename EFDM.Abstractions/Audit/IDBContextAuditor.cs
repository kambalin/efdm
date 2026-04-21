using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace EFDM.Abstractions.Audit
{
    public interface IDBContextAuditor
    {
        bool Enabled { get; set; }
        ConcurrentDictionary<string, byte> GlobalIgnoredProperties { get; }
        ConcurrentDictionary<Type, byte> IncludedTypes { get; }
        ConcurrentDictionary<Type, HashSet<string>> IgnoredTypeProperties { get; }
        Task<int> SaveChangesAsync(Func<Task<int>> baseSaveChanges, CancellationToken cancellationToken = default);
        Func<IAuditEvent, IEventEntry, object, Task> GetMapperEventAction(Type type);
        Type GetEventType(Type type);
        Type GetPropertyType(Type type);
        void ExcludeProperty<T>(Expression<Func<T, object>> propertySelector);
        void Map<TSourceEntity, TAuditEventEntity, TAuditPropertyEntity>(
            Action<IAuditEvent, IEventEntry, TAuditEventEntity> eventAction);
        void SetEventCommonAction<T>(Func<IAuditEvent, IEventEntry, T, Task> entityAction);
        void ExcludeTypeStateActions<TSourceEntity>(List<int> actions);
        /// <summary>
        /// Enqueue an audit entity to be persisted after the main SaveChanges completes.
        /// Use this to avoid performing nested SaveChanges during ChangeTracker processing.
        /// </summary>
        /// <param name="entity">Audit entity instance to persist later.</param>
        void EnqueueAuditEntity(object entity, object parent = null);
    }
}
