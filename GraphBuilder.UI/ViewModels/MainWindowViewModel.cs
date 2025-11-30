using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GraphBuilder.Core.Models;
using GraphBuilder.Core.Services;

namespace GraphBuilder.UI.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly IFunctionParser _functionParser;
    private string _currentExpression = string.Empty;
    
    public ObservableCollection<Function> Functions { get; } = new();
    
    public string CurrentExpression
    {
        get => _currentExpression;
        set
        {
            if (_currentExpression != value)
            {
                _currentExpression = value;
                OnPropertyChanged();
            }
        }
    }
    
    public MainWindowViewModel()
    {
        _functionParser = new MathExpressionParser();
    }
    
    public void AddFunction()
    {
        if (!string.IsNullOrWhiteSpace(CurrentExpression))
        {
            Functions.Add(new Function { Expression = CurrentExpression });
            CurrentExpression = string.Empty;
        }
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}