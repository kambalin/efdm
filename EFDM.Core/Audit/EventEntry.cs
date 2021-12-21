using EFDM.Abstractions.Audit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EFDM.Core.Audit {

    public class EventEntry : IEventEntry {
        public string Schema { get; set; }
        public string Table { get; set; }
        public string Name { get; set; }
        public int Action { get; set; }
        public List<IEventEntryChange> Changes { get; set; }
        public IDictionary<string, object> ColumnValues { get; set; }
        public bool Valid { get; set; }
        public List<string> ValidationResults { get; set; }
        [JsonIgnore]
        public Type EntityType { get; set; }
        [JsonIgnore]
        internal Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry Entry { get; set; }
        public Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry GetEntry() {
            return Entry;
        }
    }
}
