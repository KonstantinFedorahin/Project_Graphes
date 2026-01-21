namespace GraphBuilder.Domain.Models;

public class Graph
{
    public string Expression { get; init; } = string.Empty;
    public IReadOnlyList<GraphPoint> Points { get; init; } = Array.Empty<GraphPoint>();
}
