using Microsoft.Extensions.DependencyInjection;

namespace GraphBuilder.UI;

public static class ServiceConfiguration
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services = GraphBuilder.Application.DependencyInjection.AddApplication(services);

        services = GraphBuilder.Infrastructure.DependencyInjection.AddInfrastructure(services);

        services.AddTransient<ViewModels.MainWindowViewModel>();

        return services;
    }
}