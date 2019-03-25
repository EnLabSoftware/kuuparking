using AgentHub.Entities.Models.Application;
using AgentHub.Entities.Models.Common;
using AgentHub.Entities.Models.KuuParking;
using AgentHub.Entities.Utilities;
using Microsoft.Practices.Unity;
using System.Web.Http;
using AgentHub.Entities.DataContext;
using AgentHub.Entities.Models;
using AgentHub.Entities.Repositories;
using AgentHub.Entities.Service;
using AgentHub.Entities.UnitOfWork;
using AgentHub.Service;
using AgentHub.Web.Controllers;
using Unity.WebApi;

namespace AgentHub.Web
{
    public class UnityConfig
    {
        #region Unity Container

        public static void RegisterComponents()
        {
            RegisterTypes(ObjectFactory.Container);

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(ObjectFactory.Container);
        }
        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            container
                .RegisterType<IDataContextAsync, AgentHubDataContext>(new PerResolveLifetimeManager()) // Make sure to use PerResolveLifetimeManager to have singleton for all objects inside the same object graph. In this case we need to have single DbContext and Unit of work instances across all repository objects per request
                .RegisterType<IUnitOfWorkAsync, UnitOfWork>(new PerResolveLifetimeManager());
            //
            // Application module
            container
                .RegisterType<IRepositoryAsync<Application>, Repository<Application>>()
                .RegisterType<IRepositoryAsync<ApplicationService>, Repository<ApplicationService>>()
                .RegisterType<IRepositoryAsync<ApplicationUserAudit>, Repository<ApplicationUserAudit>>();
            //
            // Common module
            container
                .RegisterType<IRepositoryAsync<Country>, Repository<Country>>()
                .RegisterType<IRepositoryAsync<State>, Repository<State>>()
                .RegisterType<IRepositoryAsync<City>, Repository<City>>()
                .RegisterType<IRepositoryAsync<District>, Repository<District>>()
                .RegisterType<IRepositoryAsync<Slot>, Repository<Slot>>()
                .RegisterType<IRepositoryAsync<UserProfile>, Repository<UserProfile>>()
                .RegisterType<IRepositoryAsync<Address>, Repository<Address>>()
                .RegisterType<IRepositoryAsync<Comment>, Repository<Comment>>()
                .RegisterType<IRepositoryAsync<PageItem>, Repository<PageItem>>()
                .RegisterType<IRepositoryAsync<Lookup>, Repository<Lookup>>();
            //
            // KuuParking module
            container
                .RegisterType<IRepositoryAsync<SlotProvider>, Repository<SlotProvider>>()
                .RegisterType<IRepositoryAsync<SlotBooking>, Repository<SlotBooking>>()
                .RegisterType<IRepositoryAsync<SlotPayment>, Repository<SlotPayment>>();
            //
            // Service layer
            container
                .RegisterType<ISlotProviderService, SlotProviderService>()
                .RegisterType<ICommonService, CommonService>()
                .RegisterType<IAuditService, AuditService>();

            container.RegisterType<AccountController>();
            container.RegisterType<ParkingSlotController>();
        }
    }
}