using System.Web.Hosting;

namespace ScheduleMaster
{
    public class ApplicationPreload : IProcessHostPreloadClient
    {
        public void Preload(string[] parameters)
        {
            HangfireBootstrapper.Instance.Start();
        }
    }
}