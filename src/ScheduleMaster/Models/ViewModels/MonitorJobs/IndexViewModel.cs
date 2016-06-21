using ScheduleMaster.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ScheduleMaster.Models.ViewModels.MonitorJobs
{
    public class IndexViewModel
    {
        [StringLength(100, ErrorMessage = "Please provide max 100 characters")]
        public string SearchTerm { get; set; }

        public IQueryable<JobConfiguration> JobConfigurations { get; set; }
    }
}