using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AgentHub.Entities.Models.Application
{
    public partial class ApplicationUserAudit: Entity
    {
        public DateTime CreatedOn { get; set; }
        public string CreatedByUserId { get; set; }
        public int ActionTypeId { get; set; }
        public string Description { get; set; }
    }
}