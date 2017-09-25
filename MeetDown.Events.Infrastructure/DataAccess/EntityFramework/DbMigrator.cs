using MeetDown.Events.Core.DataAccess;
using MeetDown.Events.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MeetDown.Events.Infrastructure.DataAccess.EntityFramework
{
    public class DbMigrator : IDbMigrator
    {
        private readonly MeetDbContext _meetDbContext;

        public DbMigrator(IConfiguration configuration)
        {
            _meetDbContext = new MeetDbContext(configuration);
        }

        public bool AllMigrationsApplied()
        {
            var applied = _meetDbContext.Database
                .GetAppliedMigrations();

            var total = _meetDbContext.Database
                .GetMigrations();

            return !total.Except(applied).Any();
        }

        public void ApplyPendingMigrations()
        {
            _meetDbContext.Database.Migrate();
        }

        public void SeedDatabase()
        {
            // TODO: needs to support case where new record was seeded after entity was already seeded
            if (!_meetDbContext.Groups.Any())
            {
                var assDir = AssemblyDirectory;
                var path = Path.Combine(assDir, "DataAccess", "Seed", "Groups.json");
                var json = File.ReadAllText(path);
                var groups = JsonConvert.DeserializeObject<List<Group>>(json);

                _meetDbContext.AddRange(groups);
                _meetDbContext.SaveChanges();
            }
        }

        private string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);

                return Path.GetDirectoryName(path);
            }
        }
    }
}
