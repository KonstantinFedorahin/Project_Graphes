using System;
using System.Collections.Generic;
using GraphBuilder.Core.Models;
using org.mariuszgromada.math.mxparser;

namespace GraphBuilder.Core.Services;

public class MathExpressionParser : IFunctionParser
{
    public Func<double, double?> Parse(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
        {
            throw new ArgumentException("The expression cannot be empty", nameof(expression));
        }

        var normalized = expression
            .Replace(" ", "")
            .Replace("**", "^");

        if (normalized.Length >= 2 && normalized.Substring(0, 2) == "y=")
        {
            normalized = normalized.Substring(2);
        }

        if (!normalized.Contains("x", StringComparison.OrdinalIgnoreCase))
        {
            if (double.TryParse(normalized, out double constant))
            {
                return x => constant;
            }
            else
            {
                throw new ArgumentException($"The expression must contain the variable 'x'. Result: '{expression}'",
                    nameof(expression));
            }
        }

        try
        {
            CheckExpressionSyntax(normalized);

            // Prepare argument 'x'
            var xArg = new Argument("x");

            // The expression is being created once (performance matter)
            var expr = new Expression(normalized, xArg);

            if (!expr.checkSyntax())
            {
                var errorMsg = GetExpressionError(expr);
                throw new ArgumentException($"Incorrect expression: {errorMsg}", nameof(expression));
            }

            xArg.setArgumentValue(0);
            var testResult = expr.calculate();
            if (double.IsNaN(testResult) || double.IsInfinity(testResult))
            {

            }

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
        catch (Exception ex) when (ex is not ArgumentException)
        {
            throw new ArgumentException($"Error parsing expression '{expression}': {ex.Message}",
                nameof(expression), ex);
        }
    }

    public IEnumerable<GraphPoint> CalculatePoints(
        Func<double, double?> function,
        double minX,
        double maxX,
        double step)
    {
        if (step <= 0)
            step = 1;

        if (minX > maxX)
        {
            throw new ArgumentException($"minX ({minX}) cannot be greater than maxX ({maxX})");
        }

        if (step > (maxX - minX))
        {
            throw new ArgumentException($"The step ({step}) is too large for the range [{minX}, {maxX}]");
        }

        for (double x = minX; x <= maxX; x += step)
        {
            var y = function(x);
            if (y.HasValue)
            {
                yield return new GraphPoint(x, y.Value);
            }
        }
    }

    private void CheckExpressionSyntax(string expression)
    {
        if (string.IsNullOrEmpty(expression))
            throw new ArgumentException("The expression is empty");

        string[] doubleOperators = { "++", "--", "**", "//", "^^", "+-", "-+", ".." };
        foreach (var op in doubleOperators)
        {
            if (expression.Contains(op))
                throw new ArgumentException($"Incorrect operator '{op}' in expression");
        }

        int bracketCount = 0;
        foreach (char c in expression)
        {
            if (c == '(') bracketCount++;
            else if (c == ')') bracketCount--;

            if (bracketCount < 0)
                throw new ArgumentException("Incorrect placement of parentheses");
        }

        if (bracketCount != 0)
            throw new ArgumentException("Not all parentheses are closed");

        foreach (char c in expression)
        {
            if (!IsValidMathChar(c))
            {
                throw new ArgumentException($"Invalid symbol '{c}' in expression");
            }
        }
    }

    private bool IsValidMathChar(char c)
    {
        return char.IsDigit(c) ||
               char.IsLetter(c) ||
               "xXyY+-*/^().,=<>!&| ".Contains(c) ||
               c == '?' || c == 'å' || c == 'Å';
    }

    private string GetExpressionError(Expression expr)
    {
        var errorMessage = expr.getErrorMessage()?.Trim();

        if (string.IsNullOrEmpty(errorMessage))
        {
            return "Unknown syntax error";
        }

        return errorMessage;
    }
}