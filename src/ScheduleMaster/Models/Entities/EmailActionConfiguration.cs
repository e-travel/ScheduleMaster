using System.ComponentModel.DataAnnotations;

namespace ScheduleMaster.Models.Entities
{
    public class EmailActionConfiguration : ActionConfiguration
    {
        [Required]
        public string From { get; set; }
        [Required]
        public string To { get; set; }
        public string CC { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public bool SmtpEnableSSL { get; set; }
    }
}