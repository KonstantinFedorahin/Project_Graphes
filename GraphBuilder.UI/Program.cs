using Avalonia;
using ReactiveUI.Avalonia;
using System;
using GraphBuilder.Application;
using GraphBuilder.Infrastructure;
using GraphBuilder.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace GraphBuilder.UI;

class Program
{
    public static IServiceProvider Services { get; private set; } = null!;

    [STAThread]
    public static void Main(string[] args)
    {
        var services = new ServiceCollection();

        services.AddApplication();
        services.AddInfrastructure();

        services.AddTransient<MainWindowViewModel>();

        Services = services.BuildServiceProvider();

        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .UseReactiveUI()
            .WithInterFont()
            .LogToTrace();
}
