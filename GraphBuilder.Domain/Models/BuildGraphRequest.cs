namespace GraphBuilder.Domain.Models;

public class BuildGraphRequest
{
    public string Expression { get; init; } = string.Empty;
    public float From { get; init; }
    public float To { get; init; }
    public float Step { get; init; }
}
