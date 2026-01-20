using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive;
using System.Runtime.CompilerServices;
using GraphBuilder.Domain.Models;
using GraphBuilder.Application.Interfaces;
using GraphBuilder.UI.Controls;
using ReactiveUI;
using Avalonia;
using Avalonia.Threading;
using System.Linq;

namespace GraphBuilder.UI.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly IFunctionParser _functionParser;
    private readonly ICalculatePoints _graphCalculator;
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
            }
        }
    }

    public MainWindowViewModel(IFunctionParser functionParser, ICalculatePoints graphCalculator)
    {
        _functionParser = functionParser;
        _graphCalculator = graphCalculator;

        ButtonCommand = ReactiveCommand.Create(() =>
        {
            // Clear previous error
            ErrorMessage = string.Empty;

            // Check if expression is empty
            if (string.IsNullOrWhiteSpace(CurrentExpression))
            {
                ErrorMessage = "Please enter a mathematical expression";
                return;
            }

            // Validate expression
            if (!_functionParser.ValidateExpression(CurrentExpression, out string validationError))
            {
                ErrorMessage = $"Invalid expression: {validationError}";
                return;
            }

            try
            {
                // Parse the expression
                var function = _functionParser.Parse(CurrentExpression);

                if (function == null)
                {
                    ErrorMessage = "Failed to parse the expression";
                    return;
                }

                // Calculate points
                var calculated = _graphCalculator.CalculatePoints(
                    function,
                    -20,
                    20,
                    0.5f
                ).ToList();

                // Check if we got any points
                if (!calculated.Any())
                {
                    ErrorMessage = "No valid points could be calculated for this expression";
                    return;
                }

                // Update points for rendering
                Points = calculated
                    .Select(p => new Point(p.X, p.Y))
                    .ToList();

                // Add to functions list if successful
                AddFunction();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }
        },
        outputScheduler: RxApp.MainThreadScheduler);
    }

    public void AddFunction()
    {
        if (!string.IsNullOrWhiteSpace(CurrentExpression) && string.IsNullOrEmpty(ErrorMessage))
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