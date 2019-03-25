using System;
using AgentHub.Entities.Models.Common;

namespace AgentHub.Entities.Models.KuuParking
{
    public partial class SlotBooking: Entity
    {
        public int BookedByUserId { get; set; }

        public virtual UserProfile BookedByUser { get; set; }

        /// <summary>
        /// Gets or sets the vehicle information such as car number
        /// </summary>
        /// <value>
        /// The vehicle information.
        /// </value>
        public string VehicleInfo { get; set; }

        public string VehicleImageFileName { get; set; }

        public DateTime BookedOn { get; set; }

        public int SlotId { get; set; }

        public virtual Slot Slot { get; set; }

        /// <summary>
        /// Gets or sets the duration of using the slot (in hour)
        /// </summary>
        /// <value>
        /// The duration of using the slot (in hour)
        /// </value>
        public decimal Duration { get; set; }

        public decimal Price { get; set; }

        public decimal TotalAmount { get; set; }

    }
}
