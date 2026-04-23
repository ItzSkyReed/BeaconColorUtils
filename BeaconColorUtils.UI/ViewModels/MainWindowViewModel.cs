using System.Threading.Tasks;
using BeaconColorUtils.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BeaconColorUtils.UI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly BeaconColorService _colorService;

    [ObservableProperty]
    public partial bool IsLoading { get; set; } = true;

    [ObservableProperty]
    public partial int SelectedTabIndex { get; set; } = 0;

    [ObservableProperty]
    public partial object? SelectedMenuItem { get; set; }

    partial void OnSelectedMenuItemChanged(object? value)
    {
        if (value is Avalonia.Controls.Control { Tag: string tag } && int.TryParse(tag, out var index))
        {
            SelectedTabIndex = index;
        }
    }

    public BestGlassSequenceViewModel NearestVm { get; }
    public GradientGeneratorViewModel GradientVm { get; }
    public DocumentationViewModel DocumentationVm { get; }

    public MainWindowViewModel(BeaconColorService beaconColorService)
    {
        _colorService = beaconColorService;
        NearestVm = new BestGlassSequenceViewModel(_colorService);
        GradientVm = new GradientGeneratorViewModel(_colorService);
        DocumentationVm = new DocumentationViewModel();

        _ = LoadAppAsync();
    }

    private async Task LoadAppAsync()
    {
        IsLoading = true;
        await _colorService.InitializeAsync();
        IsLoading = false;
    }
}