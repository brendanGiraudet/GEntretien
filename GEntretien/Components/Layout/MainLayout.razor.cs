using GEntretien.Application.Services;
using Microsoft.AspNetCore.Components;

namespace GEntretien.Components.Layout;

public partial class MainLayout
{
    [Inject]
    private IVersionService _versionService { get; set; } = default!;

    private string _version = string.Empty;
    private bool _isMenuOpen;

    protected override void OnInitialized()
    {
        _version = _versionService.GetVersion();
    }

    private void ToggleMenu()
    {
        _isMenuOpen = !_isMenuOpen;
    }
}
