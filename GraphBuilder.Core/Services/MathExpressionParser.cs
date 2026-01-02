using System;
using System.Collections.Generic;
using GraphBuilder.Core.Models;
using org.mariuszgromada.math.mxparser;

namespace GraphBuilder.Core.Services;

public class MathExpressionParser : IFunctionParser
{
    public Func<double, double?> Parse(string expression)
    {
        var normalized = expression
            .Replace(" ", "")
            .Replace("**", "^");
        
        if (normalized.Substring(0, 2) == "y=")
        {
            normalized = normalized.Substring(2);
        }

        // Prepare argument 'x'
        var xArg = new Argument("x");

        // The expression is being created once (performance matter)
        var expr = new Expression(normalized, xArg);

        return x =>
        {
            try
            {
                xArg.setArgumentValue(x);
                var result = expr.calculate();

                if (double.IsNaN(result) || double.IsInfinity(result))
                    return null;

                return result;
            }
            catch
            {
                return null;
            }
        };
    }

    public IEnumerable<GraphPoint> CalculatePoints(
        Func<double, double?> function,
        double minX,
        double maxX,
        double step)
    {
        if (step <= 0)
            step = 1;

        for (double x = minX; x <= maxX; x += step)
        {
            var y = function(x);
            if (y.HasValue)
            {
                yield return new GraphPoint(x, y.Value);
            }
        }
    }
}
