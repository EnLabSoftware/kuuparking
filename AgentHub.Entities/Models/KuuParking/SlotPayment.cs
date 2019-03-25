using System;

namespace AgentHub.Entities.Models.KuuParking
{
    public partial class SlotPayment: Entity
    {
        public int SlotId { get; set; }

        public virtual Slot Slot { get; set; }

        public int SlotBookingId { get; set; }

        public virtual SlotBooking SlotBooking { get; set; }

        public DateTime PaidOn { get; set; }

        public decimal PayAmount { get; set; }

    }
}
