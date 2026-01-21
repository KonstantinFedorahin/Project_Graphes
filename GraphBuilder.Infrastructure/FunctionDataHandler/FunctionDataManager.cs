using GraphBuilder.Domain.Models;

namespace GraphBuilder.Infrastructure.FunctionDataHandler;

public class CalculationResult
{
    public double Start { get; set; }
    public double End { get; set; }
    public double Step { get; set; }
    public List<GraphPoint> Values { get; set; } = new List<GraphPoint>();
}

public interface IManager
{
    void Save(CalculationResult result, string filePath);

    void Load(string filePath);

    string FormatName { get; }
}

public class FunctionDataManager
{
    private IManager _manager;

    public FunctionDataManager(IManager saver)
    {
        _manager = saver;
    }

    public void SetManager(IManager saver)
    {
        _manager = saver;
    }

    public void IsManagerChosen()
    {
        if (_manager == null) throw new InvalidOperationException("Формат не выбран.");
    }

    public void Export(CalculationResult result, string path)
    {
        IsManagerChosen();
        _manager.Save(result, path);
    }

    public void Import(CalculationResult result, string path)
    {
        IsManagerChosen();
        _manager.Load(path);
    }
}
