using System.ComponentModel.DataAnnotations;

namespace AgentHub.Entities.Models.Common
{
    public partial class State: Entity
    {
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(5)]
        public string PhoneCallCode { get; set; }

        [MaxLength(10)]
        public string ReferenceCode { get; set; }

        public int? CountryId { get; set; }

        public virtual Country Country { get; set; }
    }
}
