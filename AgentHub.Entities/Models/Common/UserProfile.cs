using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AgentHub.Entities.Utilities;

namespace AgentHub.Entities.Models.Common
{
    public enum UserProfileType
    {
        BusinessOwner,
        Provider,
        Consumer
    }

    public partial class UserProfile: Entity
    {
        [MaxLength(50)]
        [Required]
        public string FirstName { get; set; }

        [MaxLength(50)]
        [Required]
        public string LastName { get; set; }

        [MaxLength(100)]
        public string Email { get; set; }

        [MaxLength(50)]
        public string PhoneNumber { get; set; }

        public UserProfileType UserProfileType { get; set; }

        public int? BillingAddressId { get; set; }

        public Address BillingAddress { get; set; }

        [MaxLength(150)]
        public string AvatarImageLocation { get; set; }

        public bool AllowCaptureVehicleIn { get; set; }
    }
}
