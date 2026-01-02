using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using GraphBuilder.UI.ViewModels;

namespace GraphBuilder.UI;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}