using GraphBuilder.Domain.Models;


namespace GraphBuilder.Domain.Interfaces;

public interface IGraphDataSerializer
{
    string Format { get; }

    void Save(Graph graph, string path);

    Graph Load(string path);
}
