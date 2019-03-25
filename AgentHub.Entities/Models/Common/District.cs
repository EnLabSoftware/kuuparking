using System.ComponentModel.DataAnnotations;

namespace AgentHub.Entities.Models.Common
{
    public partial class District: Entity
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(10)]
        public string ReferenceCode { get; set; }

        public int? CityId { get; set; }

        public virtual City City { get; set; }
    }
}
