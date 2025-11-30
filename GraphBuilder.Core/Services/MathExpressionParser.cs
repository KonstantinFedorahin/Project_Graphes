using System.Text.RegularExpressions;
using GraphBuilder.Core.Models;

namespace GraphBuilder.Core.Services;

public class MathExpressionParser : IFunctionParser
{
    public Func<double, double?> Parse(string expression)
    {
        // Упрощенный парсер - в реальном проекте используйте библиотеки вроде NCalc
        var normalized = expression.ToLower()
            .Replace(" ", "")
            .Replace("^", "**");
            
        return x =>
        {
            try
            {
                var expr = normalized.Replace("x", $"({x})");
                // Здесь должна быть реальная логика парсинга
                // Для начала можно использовать простой eval или подключить NCalc
                return EvaluateSimpleExpression(expr);
            }
            catch
            {
                return null; // Функция не определена в точке
            }
        };
    }

    public IEnumerable<GraphPoint> CalculatePoints(
        Func<double, double?> function, 
        double minX, 
        double maxX, 
        double step)
    {
        for (double x = minX; x <= maxX; x += step)
        {
            var y = function(x);
            if (y.HasValue && !double.IsInfinity(y.Value) && !double.IsNaN(y.Value))
            {
                yield return new GraphPoint(x, y.Value);
            }
        }
    }

    private double? EvaluateSimpleExpression(string expression)
    {
        // Заглушка - в реальном проекте используйте:
        // - NCalc
        // - DynamicExpresso
        // - Roslyn для компиляции выражений
        return expression.Length; // временная заглушка
    }
}