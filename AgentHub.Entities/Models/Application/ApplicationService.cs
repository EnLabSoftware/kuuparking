using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Security;

namespace AgentHub.Entities.Models.Application
{
    public class ApplicationService : Entity
    {
        [Required]
        [MaxLength(50)]
        public string ServiceKey { get; set; }

        [MaxLength(100)]
        public string ServiceName { get; set; }

        [MaxLength(250)]
        public string ServiceDescription { get; set; }

        [Required]
        [MaxLength(1024)]
        public string AllowedDomains { get; set; }

        /// <summary>
        /// Gets or sets the service culture info code. It could be vi-VN for Vietnam, en-US for USA, etc.
        /// </summary>
        public string CultureInfoCode { get; set; }

        public int ApplicationId { get; set; }

        public virtual Application Application { get; set; }
    }
}