using System.Collections.Generic;
using System.Threading.Tasks;
using AgentHub.Entities.Models.KuuParking;
using AgentHub.Entities.ViewModels;

namespace AgentHub.Entities.Service
{
    public interface ISlotProviderService: IBaseService<SlotProvider>
    {
        SlotProvider GetSlotProvider(int slotProviderId);

        Task<SlotBooking> BookSlot(SlotBooking slotBooking, int fromSlotProviderId);

        Task<bool> ReleaseSlot(int slotBookingId);

        Task<List<SlotViewModel>> Slots(int slotProviderId);

        Task<Slot> AddSlot(Slot newSlot);

        Task<bool> DeleteSlotProvider(int slotProviderId);

        Task<SlotProvider> UpdateSlotProvider(SlotProvider slotProvider);

        Task<Slot> UpdateSlot(Slot slot);

        Task<List<SlotProviderViewModel>> SearchSlotProviders(SlotSearchCriteriaViewModel criteria);

        Task<List<SlotProviderViewModel>> SearchSlotProviders(double latitude, double longitude, double radius);

        Task<List<SlotProviderLocation>> GetProviderLocationsByIP(string clientIP, bool localProviders);

        Task<List<SlotProviderLocation>> GetProviderLocations(double latitude, double longitude);

        Task<List<SlotProvider>> GetSlotProviders(int userProfileId);

        Task<List<SlotProvider>> GetInactiveSlotProviders();

        Task<bool> SetSlotStatus(int slotId, bool isAvailable);

        Task<SlotProvider> AddSlotProvider(SlotProvider slotProvider);

        Task<bool> DeleteSlot(int slotId);

        Task<Slot> AddSlot(int slotProviderId);

        Task<SlotBooking> GetSlotBooking(int slotBookingId);

        Task<bool> UpdateSlotBooking(SlotBooking slotBooking);

        Task<List<SlotProvider>> GetSlotProviders(int? stateId, int? cityId);
        Task<List<ProviderInState>> GetTotalProvidersInState();
    }
}
