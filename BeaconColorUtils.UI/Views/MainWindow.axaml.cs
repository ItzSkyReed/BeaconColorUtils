using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using FluentAvalonia.UI.Windowing;

namespace BeaconColorUtils.UI.Views;

public partial class MainWindow : AppWindow
{
    public MainWindow()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            TransparencyLevelHint = [WindowTransparencyLevel.AcrylicBlur, WindowTransparencyLevel.Mica, WindowTransparencyLevel.None];
            ExtendClientAreaToDecorationsHint = true;
            ExtendClientAreaTitleBarHeightHint = -1;
            ExtendClientAreaChromeHints = Avalonia.Platform.ExtendClientAreaChromeHints.PreferSystemChrome;

            if (TitleBar == null) return;
            TitleBar.ExtendsContentIntoTitleBar = true;
            TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;
        }
        else
        {
            TransparencyLevelHint = [WindowTransparencyLevel.None];
            ExtendClientAreaToDecorationsHint = false;
        }
        InitializeComponent();
        RootGrid.Margin = new Thickness(0, TitleBar.Height, 0, 0);
    }
}