using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgentHub.Entities.Models.Application
{
    public partial class ApplicationStatistic: Entity
    {
        public int NumberOfVisits { get; set; }
        public int NumberOfProviderDetailQueries { get; set; }
        public int NumberOfSlotBookings { get; set; }

        public int ApplicationServiceId { get; set; }

        public virtual ApplicationService ApplicationService { get; set; }
    }
}
