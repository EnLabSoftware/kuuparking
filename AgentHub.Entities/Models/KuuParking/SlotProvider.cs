using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AgentHub.Entities.Models.Common;

namespace AgentHub.Entities.Models.KuuParking
{
    public partial class SlotProvider: Entity
    {
        public int SlotOwnerId { get; set; }
        public virtual UserProfile SlotOwner { get; set; }
        [MaxLength(250)]
        public string Name { get; set; }
        [MaxLength(1024)]
        public string Description { get; set; }
        [DefaultValue(1)]
        public int MaximumSlots { get; set; }
        [DefaultValue(0)]
        public decimal Price { get; set; }
        public decimal? Rating { get; set; }
        public int AddressId { get; set; }
        public virtual Address Address { get; set; }
        [DefaultValue(0)]
        public double Latitude { get; set; }
        [DefaultValue(0)]
        public double Longitude { get; set; }
        [MaxLength(150)]
        public string ThumbnailImageLocation { get; set; }
        [MaxLength(150)]
        public string ImageLocation1 { get; set; }
        [MaxLength(150)]
        public string ImageLocation2 { get; set; }
        [MaxLength(150)]
        public string ImageLocation3 { get; set; }
        public DateTime? AvailableFromTime { get; set; }
        public DateTime? AvailableToTime { get; set; }
        [DefaultValue(true)]
        public bool? IsAvailable { get; set; }
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }
        public virtual List<Slot> Slots { get; set; }
        [DefaultValue(true)]
        public bool? IsOpen247 { get; set; }
        [DefaultValue(false)]
        public bool? IsWeekendAvailable { get; set; }
        [DefaultValue(false)]
        public bool? IsCoveredParking { get; set; }
        [DefaultValue(true)]
        public bool? IsOvernightParking { get; set; }
        [DefaultValue(true)]
        public bool? IsBusParking { get; set; }
        [DefaultValue(false)]
        public bool? IsCarWashingServiceAvailable { get; set; }
        [DefaultValue(true)]
        public bool? IsMondayAvailable { get; set; }
        public int? MondayOpenTime { get; set; }
        public int? MondayClosedTime { get; set; }
        [DefaultValue(true)]
        public bool? IsTuesdayAvailable { get; set; }
        public int? TuesdayOpenTime { get; set; }
        public int? TuesdayClosedTime { get; set; }
        [DefaultValue(true)]
        public bool? IsWednesdayAvailable { get; set; }
        public int? WednesdayOpenTime { get; set; }
        public int? WednesdayClosedTime { get; set; }
        [DefaultValue(true)]
        public bool? IsThursdayAvailable { get; set; }
        public int? ThursdayOpenTime { get; set; }
        public int? ThursdayClosedTime { get; set; }
        [DefaultValue(true)]
        public bool? IsFridayAvailable { get; set; }
        public int? FridayOpenTime { get; set; }
        public int? FridayClosedTime { get; set; }
        [DefaultValue(true)]
        public bool? IsSaturdayAvailable { get; set; }
        public int? SaturdayOpenTime { get; set; }
        public int? SaturdayClosedTime { get; set; }
        [DefaultValue(true)]
        public bool? IsSundayAvailable { get; set; }
        public int? SundayOpenTime { get; set; }
        public int? SundayClosedTime { get; set; }
        [DefaultValue(false)]
        public bool? IsPublic { get; set; }
        [DefaultValue(false)]
        public bool IsActivated { get; set; }
    }
}
