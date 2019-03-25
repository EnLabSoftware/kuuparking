using System.ComponentModel.DataAnnotations;

namespace AgentHub.Entities.Models.Common
{
    public partial class Country: Entity
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(5)]
        public string PhoneCallCode { get; set; }

        [MaxLength(10)]
        public string ReferenceCode { get; set; }
    }
}
