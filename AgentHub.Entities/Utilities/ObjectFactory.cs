using Microsoft.Practices.Unity;

namespace AgentHub.Entities.Utilities
{
    public static class ObjectFactory
    {
        private static UnityContainer container;

        public static UnityContainer Container
        {
            get { return container ?? (container = new UnityContainer()); }
        }

        public static T GetInstance<T>()
        {
            return Container.Resolve<T>();
        }
    }
}
