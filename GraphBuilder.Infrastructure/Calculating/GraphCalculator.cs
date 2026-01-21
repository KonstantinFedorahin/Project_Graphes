using DynamicData;
using GraphBuilder.Domain.Interfaces;
using GraphBuilder.Domain.Models;


namespace GraphBuilder.Infrastructure.Calculating;

public class GraphCalculator : ICalculatePoints
{
    public IEnumerable<GraphPoint> CalculatePoints(
        Func<double, double?> function,
        double minX,
        double maxX,
        double step)
    {
        if (step <= 0)
            step = 0.1;

        if (maxX <= minX)
        {
            (minX, maxX) = (maxX, minX);
        }

        // Limit the range for safety
        if (maxX - minX > 1000)
        {
            step = Math.Max(step, (maxX - minX) / 1000);
        }

        for (double x = minX; x <= maxX + step / 2; x += step)
        {
            var y = function(x);
            if (y.HasValue)
            {
                // Filter out extreme values
                if (Math.Abs(y.Value) < 1e6)
                {
                    yield return new GraphPoint(x, y.Value);
                }
            }
        }
    }
}