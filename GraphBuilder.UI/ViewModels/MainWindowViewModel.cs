using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive;
using System.Runtime.CompilerServices;
using GraphBuilder.Core.Models;
using GraphBuilder.Core.Services;
using GraphBuilder.UI.Controls;
using ReactiveUI;
using Avalonia;
using Avalonia.Threading;
using System.Linq;
using System.Reactive.Linq;

namespace GraphBuilder.UI.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly IFunctionParser _functionParser;
    private string _currentExpression = string.Empty;
    private string _errorMessage = string.Empty;

    private IReadOnlyList<Point>? _points;
    public IReadOnlyList<Point>? Points
    {
        get => _points;
        set
        {
            _points = value;
            OnPropertyChanged();
        }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            if (_errorMessage != value)
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }
    }

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
                ClearError();
            }
        }
    }

    public MainWindowViewModel()
    {
        _functionParser = new MathExpressionParser();

        ButtonCommand = ReactiveCommand.Create(() =>
        {
            try
            {
                ErrorMessage = string.Empty;

                if (string.IsNullOrWhiteSpace(CurrentExpression))
                {
                    ErrorMessage = "Enter a mathematical expression";
                    return;
                }

                var function = _functionParser.Parse(CurrentExpression);

                var calculated = _functionParser.CalculatePoints(
                    function,
                    -20,
                    5,
                    0.5f
                );

                // Here you need to pass the points into Coordinate Plane class to render a function.
                Points = calculated
                    .Select(p => new Point(p.X, p.Y))
                    .ToList();

                if (Points != null && Points.Count > 0)
                {
                    AddFunction();
                }
            }
            catch (ArgumentException ex)
            {
                ErrorMessage = ex.Message;
                Points = null;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
                Points = null;
            }
        },

        outputScheduler: RxApp.MainThreadScheduler);
    }

    public void AddFunction()
    {
        if (!string.IsNullOrWhiteSpace(CurrentExpression))
        {
            if (!Functions.Any(f => f.Expression == CurrentExpression))
            {
                Functions.Add(new Function { Expression = CurrentExpression });
            }
            CurrentExpression = string.Empty;
            ErrorMessage = string.Empty;
        }
    }

    private void ClearError()
    {
        ErrorMessage = string.Empty;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}