using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using GraphBuilder.Domain.Models;
using GraphBuilder.Domain.Interfaces;
using org.mariuszgromada.math.mxparser;

namespace GraphBuilder.Infrastructure.Parsing;

public class MathExpressionParser : IFunctionParser
{
    public Func<double, double?> Parse(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
        {
            return x => null;
        }

        var normalized = NormalizeExpression(expression);

        if (!IsValidExpression(normalized, out string validationError))
        {
            Console.WriteLine($"Validation error: {validationError}");
            return x => null;
        }

        try
        {
            // Handle constant numbers
            if (IsConstantNumber(normalized))
            {
                if (double.TryParse(normalized, out double constant))
                {
                    return x => constant;
                }
                return x => null;
            }

            // Handle identity function f(x) = x
            if (normalized == "x")
            {
                return x => x;
            }

            // Prepare argument and expression
            var xArg = new Argument("x");
            var expr = new Expression(normalized, xArg);

            // Check syntax with mXparser
            if (!expr.checkSyntax())
            {
                var error = expr.getErrorMessage();
                Console.WriteLine($"Syntax error: {error}");
                return x => null;
            }

            return x =>
            {
                try
                {
                    xArg.setArgumentValue(x);
                    var result = expr.calculate();

                    if (double.IsNaN(result) ||
                        double.IsInfinity(result) ||
                        double.IsInfinity(-result))
                        return null;

                    return result;
                }
                catch
                {
                    return null;
                }
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Parsing error for '{expression}': {ex.Message}");
            return x => null;
        }
    }

    private string NormalizeExpression(string expression)
    {
        var normalized = expression
            .Replace(" ", "")
            .Replace("**", "^")
            .Replace(",", ".")
            .ToLower();

        // Remove function prefixes if present
        if (normalized.StartsWith("y=") || normalized.StartsWith("f(x)="))
        {
            normalized = normalized.Substring(normalized.IndexOf('=') + 1);
        }

        return normalized;
    }

    private bool IsValidExpression(string expression, out string errorMessage)
    {
        errorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(expression))
        {
            errorMessage = "Expression cannot be empty";
            return false;
        }

        // Check for forbidden characters
        var forbidden = new[] { ';', '{', '}', '[', ']', '`', '~', '|', '\\' };
        if (expression.IndexOfAny(forbidden) >= 0)
        {
            errorMessage = "Expression contains forbidden characters";
            return false;
        }

        // Check bracket balance
        int bracketCount = 0;
        foreach (char c in expression)
        {
            if (c == '(') bracketCount++;
            if (c == ')') bracketCount--;
            if (bracketCount < 0)
            {
                errorMessage = "Mismatched parentheses: closing bracket without opening";
                return false;
            }
        }

        if (bracketCount != 0)
        {
            errorMessage = "Mismatched parentheses: not all brackets are closed";
            return false;
        }

        return true;
    }

    private bool IsConstantNumber(string expression)
    {
        // Check for integer numbers
        if (int.TryParse(expression, out _))
            return true;

        // Check for decimal numbers
        if (double.TryParse(expression, out _))
            return true;

        // Check for numbers with leading minus
        if (expression.StartsWith("-"))
        {
            return double.TryParse(expression.Substring(1), out _);
        }

        // Check for decimal numbers without leading zero (.5)
        if (expression.StartsWith(".") || expression.StartsWith("-."))
        {
            string number = expression.StartsWith("-.") ? expression.Substring(2) : expression.Substring(1);
            return double.TryParse(number, out _);
        }

        return false;
    }

    // Public validation method for UI
    public bool ValidateExpression(string expression, out string errorMessage)
    {
        errorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(expression))
        {
            errorMessage = "Expression cannot be empty";
            return false;
        }

        var normalized = NormalizeExpression(expression);

        if (!IsValidExpression(normalized, out errorMessage))
        {
            return false;
        }

        try
        {
            // Simple expressions are always valid
            if (IsConstantNumber(normalized) || normalized == "x")
            {
                return true;
            }

            // Check with mXparser for complex expressions
            var xArg = new Argument("x");
            var expr = new Expression(normalized, xArg);

            if (!expr.checkSyntax())
            {
                errorMessage = expr.getErrorMessage();
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            errorMessage = $"Validation error: {ex.Message}";
            return false;
        }
    }
}