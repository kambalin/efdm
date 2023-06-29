using EFDM.Core.DAL.Repositories;
using EFDM.Core.Models.Domain;
using EFDM.Test.DAL.Providers;
using System;

namespace EFDM.Test.DAL.Repositories
{
    public class TestRepository<TEntity, TKey> : Repository<TEntity, TKey>
        where TEntity : IdKeyEntityBase<TKey>, new()
        where TKey : IComparable, IEquatable<TKey>
    {
        #region fields & properties

        public override TestDatabaseContext Context { get; }

        #endregion fields & properties

        #region constructors

        public TestRepository(TestDatabaseContext dbContext) : base(dbContext)
        {
            Context = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            InitDbSet();
        }

        #endregion constructors
    }
}
