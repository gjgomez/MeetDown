using System;
using System.Collections.Generic;
using System.Text;

namespace MeetDown.Events.Core.DataAccess
{
    public interface IDbMigrator
    {
        bool AllMigrationsApplied();

        void ApplyPendingMigrations();

        void SeedDatabase();
    }
}
