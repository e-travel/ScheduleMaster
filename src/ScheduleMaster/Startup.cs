using System.Linq;
using ScheduleMaster.DataAccess;
using ScheduleMaster.Models.Entities;
using Hangfire;
using Microsoft.Owin;
using Owin;
using NLog;

[assembly: OwinStartup(typeof(ScheduleMaster.Startup))]
namespace ScheduleMaster
{
    public partial class Startup
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public void Configuration(IAppBuilder app)
        {
            //just to create db very quick 
            using (var db = new ScheduleMasterContext())
            {
                var a = db.ActionConfigurations.OfType<EmailActionConfiguration>().FirstOrDefault();
            }

            //Hangfire
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                AuthorizationFilters = new[] { new HangfireRestrictiveAuthorizationFilter() }
            });
        }
    }
}
