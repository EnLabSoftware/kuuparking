using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgentHub.Entities.Models.Application;
using AgentHub.Entities.Models.Common;
using AgentHub.Entities.Repositories;
using AgentHub.Entities.Service;
using AgentHub.Entities.UnitOfWork;
using AgentHub.Entities.Utilities;

namespace AgentHub.Service
{

    public class AuditService: BaseService<ApplicationUserAudit>, IAuditService
    {
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        public AuditService(IRepositoryAsync<ApplicationUserAudit> repository, IUnitOfWorkAsync unitOfWorkAsync)
            : base(repository)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
        }

        public void UserLogin(string userId)
        {
            base.Insert(new ApplicationUserAudit()
            {
                ActionTypeId = (int) UserActionTypes.Login,
                CreatedByUserId = userId,
                CreatedOn = DateTime.Now
            });
            _unitOfWorkAsync.SaveChanges();
        }

        public static void AuditUserLogin(string userId)
        {
            Task.Factory.StartNew(() =>
            {
                var auditService = ObjectFactory.GetInstance<IAuditService>();
                auditService.UserLogin(userId);
            });
        }

    }
}
