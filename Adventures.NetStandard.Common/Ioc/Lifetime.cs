using Unity.Lifetime;

namespace Adventures.NetStandard.Common.Ioc
{
    public static class Lifetime
    {
        public static ContainerControlledLifetimeManager Singleton =>
            new ContainerControlledLifetimeManager();
    }
}
