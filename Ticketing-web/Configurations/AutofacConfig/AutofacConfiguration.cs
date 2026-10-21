using Autofac;
using Autofac.Extensions.DependencyInjection;
using Ticketing_web.Configurations.Interfaces.Interfaces;
using Ticketing_web.Services.Http;

namespace Ticketing_web.Configurations.AutofacConfig;

public static class AutofacConfiguration
{
    public static ConfigureHostBuilder AddAutofac(
        this ConfigureHostBuilder builder,
        string baseAddress)
    {
        builder.UseServiceProviderFactory(new AutofacServiceProviderFactory());
        builder.ConfigureContainer<ContainerBuilder>(option =>
        {
            option.RegisterModule(new AutofacModule(baseAddress));
        });

        return builder;
    }
}
public class AutofacModule : Module
{
    private readonly string _baseAddress;
    public AutofacModule(string baseAddress)
    {
        _baseAddress = baseAddress;
    }

    protected override void Load(ContainerBuilder _)
    {

        var assembly = System.Reflection
            .Assembly
            .GetAssembly(typeof(AutofacConfiguration));
        var baseAddress = _baseAddress;

        if (assembly != null)
        {
            var serviceTypes = assembly.GetTypes()
                .Where(t => typeof(IService)
                .IsAssignableFrom(t) &&
                    t.IsInterface &&
                    t != typeof(IService)
                    ).ToList();
            foreach (var serviceType in serviceTypes)
            {
                var implementationType = assembly.GetTypes()
                    .FirstOrDefault(t =>
                    !t.IsInterface && serviceType
                    .IsAssignableFrom(t));
                if (implementationType != null)
                {
                    _.RegisterType(implementationType)
                        .As(serviceType)
                        .WithParameter("baseAddress", baseAddress)
                        .InstancePerLifetimeScope();
                }
            }
            _.RegisterType<HttpContextAccessor>()
                .As<IHttpContextAccessor>()
                .SingleInstance();
        }


        _.RegisterType<ApiClient>()
          .AsSelf()
          .InstancePerLifetimeScope();
        _.Register(c =>
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(baseAddress),
            };
            return client;
        })
            .As<HttpClient>()
            .InstancePerLifetimeScope();


        base.Load(_);
    }

}
