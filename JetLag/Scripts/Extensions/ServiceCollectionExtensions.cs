using JetLag.Scripts.Factory;


namespace JetLag.Scripts.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterFactory<TInterface, TFactory>(
            this IServiceCollection services
        )
            where TInterface : class
            where TFactory : class, IFactory<TInterface>
        {
            services.AddTransient<TFactory>();
            services.AddScoped(sp => sp.GetRequiredService<TFactory>().Create());

            return services;
        }
    }
}
