using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgentHub.Entities.Models.Common
{
    public partial class Lookup: Entity
    {
        public int LookupType { get; set; }

        [MaxLength(50)]
        public string Key { get; set; }
        
        public string Value { get; set; }
        
        public int? ParentId { get; set; }

        [DefaultValue(false)]
        public bool IsDeleted { get; set; }
    }

    public enum UserActionTypes
    {
        Login = 1,
        UpdateProfile = 2,
        ActivateAccount = 3
    }
}
