using GraphBuilder.Domain.Interfaces;
using GraphBuilder.Domain.Models;
using System.IO;
using System.Text;
using System.Globalization;


namespace GraphBuilder.Infrastructure.GraphSerializer;

public class TxtGraphSerializer : IGraphDataSerializer
{
    public string Format => "txt";

    public void Save(Graph graph, string path)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine($"Expression: {graph.Expression}");
        sb.AppendLine("Points:");
        sb.AppendLine("X\tY");
        
        foreach (var point in graph.Points)
        {
            sb.AppendLine($"{point.X}\t{point.Y}");
        }
        
        File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
    }

    public Graph Load(string path)
    {
        var lines = File.ReadAllLines(path, Encoding.UTF8);
        
        string expression = string.Empty;
        var points = new List<GraphPoint>();
        
        bool readingPoints = false;
        
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;
                
            if (line.StartsWith("Expression:", StringComparison.OrdinalIgnoreCase))
            {
                expression = line.Substring("Expression:".Length).Trim();
                continue;
            }
            
            if (line.Equals("X\tY", StringComparison.OrdinalIgnoreCase))
            {
                readingPoints = true;
                continue;
            }
            
            if (readingPoints)
            {
                var parts = line.Split('\t', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2 && 
                    double.TryParse(parts[0], CultureInfo.InvariantCulture, out double x) &&
                    double.TryParse(parts[1], CultureInfo.InvariantCulture, out double y))
                {
                    points.Add(new GraphPoint(x, y));
                }
            }
        }
        
        return new Graph
        {
            Expression = expression,
            Points = points
        };
    }
}
