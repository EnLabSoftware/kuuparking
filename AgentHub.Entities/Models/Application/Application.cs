using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgentHub.Entities.Models.Application
{
    public class Application: Entity
    {
        [Required]
        [MaxLength(50)]
        public string ApplicationKey { get; set; }

        [MaxLength(100)]
        public string ApplicationName { get; set; }

        [MaxLength(250)]
        public string ApplicationDescription { get; set; }

        public DateTime RequestedOn { get; set; }

        public DateTime ActivatedOn { get; set; }

        public virtual ICollection<ApplicationService> ApplicationServices { get; set; }
    }
}