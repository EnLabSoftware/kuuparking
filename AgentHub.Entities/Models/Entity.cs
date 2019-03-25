using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AgentHub.Entities.Infrastructure;

namespace AgentHub.Entities.Models
{
    public abstract class Entity : IObjectState
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [NotMapped]
        public ObjectState ObjectState { get; set; }

        protected Entity()
        {
            //ObjectState = ObjectState.Added;            
        }
    }
}