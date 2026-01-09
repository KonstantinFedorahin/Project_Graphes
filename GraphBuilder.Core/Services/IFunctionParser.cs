using GraphBuilder.Core.Models;

namespace GraphBuilder.Core.Services;

public interface IFunctionParser
{
    Func<double, double?> Parse(string expression);

    IEnumerable<GraphPoint> CalculatePoints(
        Func<double, double?> function,
        double minX,
        double maxX,
        double step);

    // New method for expression validation
    bool ValidateExpression(string expression, out string errorMessage);
}