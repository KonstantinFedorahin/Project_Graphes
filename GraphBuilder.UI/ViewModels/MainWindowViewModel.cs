using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive;
using System.Runtime.CompilerServices;
using GraphBuilder.Domain.Models;
using GraphBuilder.Domain.Interfaces;
using GraphBuilder.UI.Controls;
using ReactiveUI;
using Avalonia;
using Avalonia.Threading;
using System.Linq;

namespace GraphBuilder.UI.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly IBuildGraphService _buildGraphService;

    private string _currentExpression = string.Empty;
    private string _errorMessage = string.Empty;
    private IReadOnlyList<Point>? _points;

    public event PropertyChangedEventHandler? PropertyChanged;

    public MainWindowViewModel(IBuildGraphService buildGraphService)
    {
        _buildGraphService = buildGraphService;

        ButtonCommand = ReactiveCommand.Create(ExecuteBuildGraph);
    }

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

    public string ErrorMessage
    {
        get => _errorMessage;
        private set
        {
            if (_errorMessage != value)
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }
    }

    public IReadOnlyList<Point>? Points
    {
        get => _points;
        private set
        {
            _points = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<Function> Functions { get; } = new();

    public ReactiveCommand<Unit, Unit> ButtonCommand { get; }
    
    private void ExecuteBuildGraph()
    {
        ErrorMessage = string.Empty;

        var result = _buildGraphService.Execute(new BuildGraphRequest
        {
            Expression = CurrentExpression,
            From = -20,
            To = 20,
            Step = 0.5f
        });

        if (!result.IsSuccess)
        {
            ErrorMessage = result.ErrorMessage!;
            return;
        }

        Points = result.Points!
            .Select(p => new Point(p.X, p.Y))
            .ToList();

        AddFunction();
    }

    public void AddFunction()
    {
        if (!string.IsNullOrWhiteSpace(CurrentExpression) && string.IsNullOrEmpty(ErrorMessage))
        {
            Functions.Add(new Function { Expression = CurrentExpression });
            CurrentExpression = string.Empty;
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}