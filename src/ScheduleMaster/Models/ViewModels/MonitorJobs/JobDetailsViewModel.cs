using ScheduleMaster.Models.Entities;

namespace ScheduleMaster.Models.ViewModels.MonitorJobs
{
    public class JobDetailsViewModel
    {
        public string SearchTerm { get; set; }
        public JobConfiguration JobConfiguration { get; set; }
    }
}