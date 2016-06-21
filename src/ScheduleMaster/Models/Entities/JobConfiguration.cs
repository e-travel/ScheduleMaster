using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ScheduleMaster.Models.Entities
{
    public class JobConfiguration
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public bool IsEnabled { get; set;   }
        [Required]
        public string SqsAccessKey { get; set; }
        [Required]
        public string SqsSecretKey { get; set; }
        [Required]
        public string SqsRegion { get; set; }
        [Required]
        [MaxLength(1024)]
        public string SqsQueueUrl { get; set; }
        [Range(1,20)]
        public int LongPollingTimeSeconds { get; set; }
        [Range(1,10)]
        public int NumberOfDequeueMessages { get; set; }
        public bool DeleteMessageAfterSuccess { get; set; }
        [Required]
        public string CronExpression { get; set; }

        public virtual IList<ActionConfiguration> ActionConfigurations { get; set; }
    }
}