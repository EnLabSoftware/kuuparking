
using System.ComponentModel.DataAnnotations.Schema;

namespace AgentHub.Entities.Infrastructure
{
    public interface IObjectState
    {
        [NotMapped]
        ObjectState ObjectState { get; set; }
    }
}