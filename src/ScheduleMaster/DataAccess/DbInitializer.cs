using ScheduleMaster.Migrations;
using System;
using System.Data.Entity;


namespace ScheduleMaster.DataAccess
{
    public class DbInitializer : IDatabaseInitializer<ScheduleMasterContext>
    {
        public void InitializeDatabase(ScheduleMasterContext context)
        {
            if (context.Database.Exists())
            {
                bool comaptible;

                try
                {
                    comaptible = context.Database.CompatibleWithModel(true);
                }
                catch
                {
                    comaptible = false;
                }

                if (!comaptible)
                {
                    var migration = new MigrateDatabaseToLatestVersion<ScheduleMasterContext, Configuration>();

                    migration.InitializeDatabase(context);
                }
            }
            else
            {
                context.Database.Create();
            }
        }
    }
}
