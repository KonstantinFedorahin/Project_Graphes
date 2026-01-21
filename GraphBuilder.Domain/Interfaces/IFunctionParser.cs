namespace GraphBuilder.Domain.Interfaces;

public interface IFunctionParser
{
    Func<double, double?> Parse(string expression);

    bool ValidateExpression(string expression, out string errorMessage);
}
