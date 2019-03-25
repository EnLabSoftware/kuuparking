using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgentHub.Entities.Models.Common
{
    public partial class Comment: Entity
    {
        public int EntityType { get; set; }
        public int EntityId { get; set; }
        public decimal? Rating { get; set; }
        [Required]
        [MaxLength(2048)]
        public string CommentDetail { get; set; }
    }
}
