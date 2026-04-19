using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace GEntretien.Components.Layout;

public partial class NavMenu
{
    [Parameter]
    public bool IsCollapsed { get; set; }

    [Parameter]
    public EventCallback<bool> OnCollapseChanged { get; set; }

    private Task OnLinkClicked()
    {
        if (OnCollapseChanged.HasDelegate)
            return OnCollapseChanged.InvokeAsync(true);

        return Task.CompletedTask;
    }
}
