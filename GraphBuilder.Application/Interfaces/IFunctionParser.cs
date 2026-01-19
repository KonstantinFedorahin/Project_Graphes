using GraphBuilder.Domain.Models;

namespace GraphBuilder.Application.Interfaces;

public interface IFunctionParser
{
    Func<double, double?> Parse(string expression);

    IEnumerable<GraphPoint> CalculatePoints(
        Func<double, double?> function,
        double minX,
        double maxX,
        double step);

    bool ValidateExpression(string expression, out string errorMessage);
}
