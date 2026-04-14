using Avalonia.Controls;
using Avalonia.Interactivity;

namespace BeaconColorUtils.UI.Views;

public partial class BestGlassSequenceView : UserControl
{
    public BestGlassSequenceView()
    {
        InitializeComponent();
    }

    private void HexInput_LostFocus(object? sender, RoutedEventArgs e)
    {
        if (DataContext is ViewModels.BestGlassSequenceViewModel vm)
        {
            vm.ValidateAndRevertHexCommand.Execute(null);
        }
    }
}