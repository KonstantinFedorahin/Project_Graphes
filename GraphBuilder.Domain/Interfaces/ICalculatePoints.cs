using GraphBuilder.Domain.Models;

namespace GraphBuilder.Domain.Interfaces;

public interface ICalculatePoints
{
    IEnumerable<GraphPoint> CalculatePoints(
        Func<double, double?> function,
        double minX,
        double maxX,
        double step);
}