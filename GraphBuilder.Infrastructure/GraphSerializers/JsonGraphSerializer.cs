using GraphBuilder.Domain.Interfaces;
using GraphBuilder.Domain.Models;
using System.IO;
using System.Text.Json;

namespace GraphBuilder.Infrastructure.GraphSerializer;

public class JsonGraphSerializer : IGraphDataSerializer
{
    public string Format => "json";

    public void Save(Graph graph, string path)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(graph, options);
        File.WriteAllText(path, json);
    }

    public Graph Load(string path)
    {
        var json = File.ReadAllText(path);

        var graph = JsonSerializer.Deserialize<Graph>(json);

        if (graph == null)
            throw new InvalidOperationException("Invalid graph data");

        return graph;
    }
}