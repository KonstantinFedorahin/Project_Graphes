using GraphBuilder.Domain.Interfaces;
using GraphBuilder.Domain.Models;
using System.Globalization;
using System.IO;
using System.Text;

namespace GraphBuilder.Infrastructure.GraphSerializers;

public class CsvGraphSerializer : IGraphDataSerializer
{
    public string Format => "csv";

    public void Save(Graph graph, string path)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"Expression,{graph.Expression}");
        sb.AppendLine("X,Y");

        foreach (var point in graph.Points)
        {
            sb.AppendLine(
                $"{point.X.ToString(CultureInfo.InvariantCulture)}," +
                $"{point.Y.ToString(CultureInfo.InvariantCulture)}");
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

            var parts = line.Split(',', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 2 && parts[0].Equals("Expression", StringComparison.OrdinalIgnoreCase))
            {
                expression = parts[1];
                continue;
            }

            if (parts[0].Equals("X", StringComparison.OrdinalIgnoreCase) &&
                parts[1].Equals("Y", StringComparison.OrdinalIgnoreCase))
            {
                readingPoints = true;
                continue;
            }

            if (readingPoints &&
                double.TryParse(parts[0], CultureInfo.InvariantCulture, out var x) &&
                double.TryParse(parts[1], CultureInfo.InvariantCulture, out var y))
            {
                points.Add(new GraphPoint(x, y));
            }
        }

        return new Graph
        {
            Expression = expression,
            Points = points
        };
    }
}
