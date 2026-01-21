using Microsoft.Extensions.DependencyInjection;
using GraphBuilder.Application.Interfaces;
using GraphBuilder.Infrastructure.Parsing;
using GraphBuilder.Infrastructure.Calculating;

namespace GraphBuilder.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IFunctionParser, MathExpressionParser>();
        services.AddSingleton<ICalculatePoints, GraphCalculator>();
        return services;
    }
}