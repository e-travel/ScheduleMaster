using System.Linq;
using ScheduleMaster.DataAccess;
using System.Data.Entity;

namespace ScheduleMaster
{
    internal static class ScheduleMasterConfig
    {
        static ScheduleMasterConfig()
        {

        }

        public static void StartUp()
        {
            using (var context = new ScheduleMasterContext())
            {
                var jobs = context.JobConfigurations.Take(10).ToList();
            }

        }
    }
}