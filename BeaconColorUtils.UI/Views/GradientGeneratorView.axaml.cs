using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using BeaconColorUtils.UI.ViewModels;

namespace BeaconColorUtils.UI.Views;

public partial class GradientGeneratorView : UserControl
{

    public GradientGeneratorView()
    {
        InitializeComponent();

        GradientRect.SizeChanged += (_, e) =>
        {
            if (DataContext is GradientGeneratorViewModel vm)
            {
                vm.PreviewWidth = e.NewSize.Width;
            }
        };
    }
    private void StepsInput_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;
        Root.Focus();
    }

    private void HexInput_LostFocus(object? sender, RoutedEventArgs e)
    {
        if (sender is Control { DataContext: ColorStopViewModel vm })
        {
            vm.ValidateAndRevertHexCommand.Execute(null);
        }
    }

}