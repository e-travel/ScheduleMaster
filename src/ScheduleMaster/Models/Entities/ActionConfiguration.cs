using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ScheduleMaster.Models.Entities
{
    public class ActionConfiguration
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public bool Active { get; set; }

        [AllowHtml]
        public string RegularExpression { get; set; }

        public virtual int JobConfigurationId { get; set; }
        public virtual JobConfiguration JobConfiguration { get; set; }
    }
}