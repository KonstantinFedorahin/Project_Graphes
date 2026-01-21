using Microsoft.Extensions.DependencyInjection;
using GraphBuilder.Domain.Interfaces;
using GraphBuilder.Infrastructure.Parsing;
using GraphBuilder.Infrastructure.Calculating;
using GraphBuilder.Domain.Services;

namespace GraphBuilder.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddTransient<IBuildGraphService, BuildGraphService>();
        services.AddSingleton<IFunctionParser, MathExpressionParser>();
        services.AddSingleton<ICalculatePoints, GraphCalculator>();
        return services;
    }
}