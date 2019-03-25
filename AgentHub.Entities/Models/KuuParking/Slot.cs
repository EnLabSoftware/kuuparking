using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgentHub.Entities.Models.KuuParking
{
    public partial class Slot: Entity
    {        
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(150)]
        public string Description { get; set; }

        public int SlotProviderId { get; set; }
       
        public decimal Price { get; set; }

        [DefaultValue(true)]
        public bool IsAvailable { get; set; }

        [NotMapped]
        public SlotBooking ActiveBooking { get; set; }
    }
}
