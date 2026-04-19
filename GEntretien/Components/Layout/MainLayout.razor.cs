using GEntretien.Application.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace GEntretien.Components.Layout;

public partial class MainLayout : LayoutComponentBase
{
    [Inject] public required IVersionService VersionService { get; set; }

    [Inject] public required ILogger<MainLayout> Logger { get; set; }

    private string _version = string.Empty;
    private bool _isMenuCollapsed;

    protected override void OnInitialized()
    {
        _version = VersionService.GetVersion();
    }

    private void ToggleMenu()
    {
        _isMenuCollapsed = !_isMenuCollapsed;
        Logger.LogInformation("ToggleMenu invoked; collapsed={Collapsed}", _isMenuCollapsed);

        StateHasChanged();
    }

    private void SetCollapsed(bool collapsed)
    {
        _isMenuCollapsed = collapsed;
        StateHasChanged();
    }
}
