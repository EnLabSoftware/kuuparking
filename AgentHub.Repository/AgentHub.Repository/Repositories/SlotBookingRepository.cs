using System.Linq;
using AgentHub.Entities.Models;
using AgentHub.Entities.Models.KuuParking;
using AgentHub.Entities.Repositories;

namespace AgentHub.Repository.Repositories
{
    public static class SlotBookingRepository
    {
        public static SlotBooking GetActiveBooking(this IRepositoryAsync<SlotBooking> repository, int slotId)
        {
            var context = repository.Context as AgentHubDataContext;
            if (context == null)
                return null;

            var activeBooking =
                context.SlotBookings.FirstOrDefault(
                    booking =>
                        booking.SlotId == slotId &&
                        context.SlotPayments.FirstOrDefault(_ => _.SlotBookingId == booking.ID) == null);
            return activeBooking;
        }

    }
}
