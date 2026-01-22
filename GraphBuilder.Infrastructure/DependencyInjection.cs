using Microsoft.Extensions.DependencyInjection;
using GraphBuilder.Domain.Interfaces;
using GraphBuilder.Infrastructure.Parsing;
using GraphBuilder.Infrastructure.Calculating;
using GraphBuilder.Domain.Services;
using GraphBuilder.Infrastructure.GraphSerializers;
using GraphBuilder.Infrastructure.GraphSerializer;

namespace GraphBuilder.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IGraphDataSerializer, CsvGraphSerializer>();
        services.AddSingleton<IGraphDataSerializer, JsonGraphSerializer>();
        services.AddSingleton<IGraphDataSerializer, TxtGraphSerializer>();
        services.AddSingleton<IBuildGraphService, BuildGraphService>();
        services.AddSingleton<IFunctionParser, MathExpressionParser>();
        services.AddSingleton<ICalculatePoints, GraphCalculator>();
        return services;
    }
}