using ScheduleMaster.Models.Entities;
using Hangfire;
using System.Globalization;

namespace ScheduleMaster.Component
{
    public static class HangfireAdapter
    {
        public static void Activate(int jobId, string cronExpression)
        {
            RecurringJob.AddOrUpdate(jobId.ToString(CultureInfo.InvariantCulture), () => new JobCommand().Execute(jobId), 
                                     cronExpression);
        }

        public static void Deactivate(int jobId)
        {
            RecurringJob.RemoveIfExists(jobId.ToString(CultureInfo.InvariantCulture));
        }

        public static void RunNow(int jobId)
        {
            RecurringJob.Trigger(jobId.ToString(CultureInfo.InvariantCulture));
        }
    }
}