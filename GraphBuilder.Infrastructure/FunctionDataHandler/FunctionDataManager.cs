using GraphBuilder.Domain.Models;
using GraphBuilder.Domain.Interfaces;

namespace GraphBuilder.Infrastructure.FunctionDataHandler;

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

    public void Import(string path)
    {
        IsManagerChosen();
        _manager.Load(path);
    }
}
