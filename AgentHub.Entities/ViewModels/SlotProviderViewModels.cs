using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using AgentHub.Entities.Models.Common;
using AgentHub.Entities.Models.KuuParking;

namespace AgentHub.Entities.ViewModels
{
    public class SlotSearchCriteriaViewModel
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double Radius { get; set; }
        public string AddressLine { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public DateTime CheckinDate { get; set; }
        public DateTime CheckinTime { get; set; }
        public decimal OccupationDuration { get; set; }
        public decimal? PriceFrom { get; set; }
        public decimal? PriceTo { get; set; }
        public bool? IsOpen247 { get; set; }
        public bool? IsWeekendAvailable { get; set; }
        public bool? IsCoveredParking { get; set; }
        public bool? IsOvernightParking { get; set; }
        public bool? IsBusParking { get; set; }
        public bool? IsCarWashingServiceAvailable { get; set; }
    }

    public class SlotProviderLocation
    {
        public string AddressLine { get; set; }
        public string Street { get; set; }
        public int? DistrictId { get; set; }
        public string District { get; set; }
        public int? CityId { get; set; }
        public string City { get; set; }
        public int? StateId { get; set; }
        public string State { get; set; }
        public int? CountryId { get; set; }
        public string Country { get; set; }
    }

    [NotMapped]
    public class SlotProviderViewModel : SlotProvider
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string AddressLine { get; set; }
        public double Distance { get; set; }
    }

    public class SlotBookingViewModel
    {
        public int SlotProviderId { get; set; }
        public int SlotId { get; set; }
        public string SlotName { get; set; }
        public int BookedByUserId { get; set; }
        public DateTime BookedOn { get; set; }
        public string VehicleInfo { get; set; }
        public string VehicleImageFileName { get; set; }
        public decimal Price { get; set; }
        public decimal Duration { get; set; }
    }

    [NotMapped]
    public class SlotViewModel : Slot
    {
        public DateTime? BookedOn { get; set; }
        
        public string VehicleInfo { get; set; }
    }

    public class SlotProvidersInPlace
    {
        public List<SlotProvider> SlotProviders { get; set; }
        public State State { get; set; }
        public City City { get; set; }
        public List<City> Cities { get; set; }
        public List<ProviderInState> ProvidersInState { get; set; }
    }

    public class ProviderInState
    {
        public int StateID { get; set; }
        public string Name { get; set; }
        public int TotalSlotProviders { get; set; }
    }

    public class ProviderInCity
    {
        public int CityID { get; set; }
        public string Name { get; set; }
        public int TotalSlotProviders { get; set; }
    }
}