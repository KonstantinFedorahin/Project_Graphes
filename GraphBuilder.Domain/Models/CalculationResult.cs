namespace GraphBuilder.Domain.Models;

public class CalculationResult
{
    public double Start { get; set; }
    public double End { get; set; }
    public double Step { get; set; }
    public List<GraphPoint> Values { get; set; } = new List<GraphPoint>();
}