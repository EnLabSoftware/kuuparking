using System.ComponentModel.DataAnnotations;

namespace AgentHub.Entities.Models.Common
{
    public partial class City : Entity
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(10)]
        public string ReferenceCode { get; set; }

        public int? StateId { get; set; }

        public virtual State State { get; set; }
    }
}
