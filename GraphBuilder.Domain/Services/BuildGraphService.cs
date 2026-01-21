using System;
using GraphBuilder.Domain.Interfaces;
using GraphBuilder.Domain.Models;

namespace GraphBuilder.Domain.Services;


public class BuildGraphService : IBuildGraphService
{
    private readonly IFunctionParser _parser;
    private readonly ICalculatePoints _calculator;

    public BuildGraphService(
        IFunctionParser parser,
        ICalculatePoints calculator)
    {
        _parser = parser;
        _calculator = calculator;
    }

    public BuildGraphResult Execute(BuildGraphRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Expression))
        {
            return Fail("Expression is empty");
        }

        if (!_parser.ValidateExpression(request.Expression, out var error))
        {
            return Fail($"Invalid expression: {error}");
        }

        var function = _parser.Parse(request.Expression);
        if (function == null)
        {
            return Fail("Failed to parse expression");
        }

        var points = _calculator
            .CalculatePoints(function, request.From, request.To, request.Step)
            .Select(p => new GraphPoint(p.X, p.Y))
            .ToList();

        if (!points.Any())
        {
            return Fail("No points calculated");
        }

        return new BuildGraphResult
        {
            IsSuccess = true,
            Points = points
        };
    }

    private static BuildGraphResult Fail(string message) =>
        new()
        {
            IsSuccess = false,
            ErrorMessage = message
        };
}
