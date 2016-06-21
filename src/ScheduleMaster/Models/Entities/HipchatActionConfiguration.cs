using System.ComponentModel.DataAnnotations;

namespace ScheduleMaster.Models.Entities
{
    public class HipchatActionConfiguration : ActionConfiguration
    {
        [Required]
        public string RoomName { get; set; }
        [Required]
        public string ApiKey { get; set; }
        /// <summary>
        /// Yellow,
        /// Green,
        /// Purple,
        /// Gray,
        /// Red,
        /// </summary>
        [RegularExpression(@"^(?i)(Yellow|Green|Purple|Gray|Red)$")]
        public string NotificationColor { get; set; }
        [StringLength(256)]
        public string Mentions { get; set; }
    }
}