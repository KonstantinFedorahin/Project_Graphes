using GraphBuilder.Domain.Models;


namespace GraphBuilder.Domain.Interfaces;

public interface IManager
{
    void Save(CalculationResult result, string filePath);

    void Load(string filePath);

    string FormatName { get; }
}