using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgentHub.Entities.Service
{
    public interface IAuditService
    {
        void UserLogin(string userId);
    }
}
