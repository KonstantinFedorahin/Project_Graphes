using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive;
using System.Runtime.CompilerServices;
using GraphBuilder.Core.Models;
using GraphBuilder.Core.Services;
using ReactiveUI;
using Avalonia.Threading;
using System.Linq;


namespace GraphBuilder.UI.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly IFunctionParser _functionParser;
    private string _currentExpression = string.Empty;

    public ReactiveCommand<Unit, Unit> ButtonCommand { get; }
    
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

        ButtonCommand = ReactiveCommand.Create(() =>
        {
            var function = _functionParser.Parse(CurrentExpression);

            var points = _functionParser.CalculatePoints(
                function,
                0,
                2,
                1
            );

            foreach(var el in points.ToList())
                Console.WriteLine(el);
        },
        outputScheduler: RxApp.MainThreadScheduler);
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