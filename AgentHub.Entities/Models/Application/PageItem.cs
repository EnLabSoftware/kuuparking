using System.ComponentModel.DataAnnotations;

namespace AgentHub.Entities.Models.Application
{
    public partial class PageItem: Entity
    {
        [MaxLength(500)]        
        public string Title { get; set; }

        [MaxLength(2048)]
        public string Description { get; set; }

        [MaxLength(500)]
        public string OGTitle { get; set; }

        [MaxLength(2048)]
        public string OGDescription { get; set; }

        [MaxLength(500)]
        public string Keywords { get; set; }

        [Required]
        [MaxLength(500)]        
        public string FriendlyUrl { get; set; }

        [Required]
        [MaxLength(50)]
        public string ControllerName { get; set; }

        [Required]
        [MaxLength(50)]
        public string ActionName { get; set; }
    }
}
