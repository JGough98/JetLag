using JetLag.Scripts.Factory.Interface;


namespace JetLag.Scripts.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterConcreteUnderMutipleTypes<TConcrete, UInterface, VInterface>(
            this IServiceCollection services
        )
            where TConcrete : class, UInterface, VInterface
            where UInterface : class
            where VInterface : class
        {
            services.AddScoped<TConcrete>();
            services.AddScoped<UInterface>(sp => sp.GetRequiredService<TConcrete>());
            services.AddScoped<VInterface>(sp => sp.GetRequiredService<TConcrete>());
            return services;
        }

        public static IServiceCollection RegisterConcreteFactory<TFactory, TInterface>(
            this IServiceCollection services
        )
            where TInterface : class
            where TFactory : class, IFactory<TInterface>
        {
            services.AddTransient<IFactory<TInterface>, TFactory>();

            return services;
        }

        public static IServiceCollection RegisterConcreteFactory<TFactory, TInterface, TArguments>(
            this IServiceCollection services
        )
            where TInterface : class
            where TFactory : class, IFactory<TInterface, TArguments>
        {
            services.AddTransient<IFactory<TInterface, TArguments>, TFactory>();

            return services;
        }

        public static IServiceCollection RegisterFactoryOutput<TFactory, TInterface>(
            this IServiceCollection services
        )
            where TInterface : class
            where TFactory : class, IFactory<TInterface>
        {
            RegisterConcreteFactory<TFactory, TInterface>(services);
            services.AddScoped(sp => sp.GetRequiredService<IFactory<TInterface>>().Create());

            return services;
        }
    }
}
