using Unity.Lifetime;

namespace CsvEditSharp.Ioc
{
    public static class Lifetime
    {
        public static ContainerControlledLifetimeManager Singleton => 
            new ContainerControlledLifetimeManager();
    }
}
