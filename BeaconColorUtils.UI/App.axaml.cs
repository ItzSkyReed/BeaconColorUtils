using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using BeaconColorUtils.UI.ViewModels;
using BeaconColorUtils.UI.Views;

using Microsoft.Extensions.DependencyInjection;
using System;

using BeaconColorUtils.Core.Cache;
using BeaconColorUtils.Core.Interfaces;
using BeaconColorUtils.Core.Services;
using BeaconColorUtils.Infrastructure.Storage;

namespace BeaconColorUtils.UI;


public partial class App : Application
{
    public IServiceProvider? Services { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IBeaconDataLoader, BeaconDataLoader>();
        services.AddSingleton<BeaconColorService>();
        services.AddTransient<MainWindowViewModel>();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        Services = services.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = Services.GetRequiredService<MainWindowViewModel>()
            };

        }

        base.OnFrameworkInitializationCompleted();
    }
}