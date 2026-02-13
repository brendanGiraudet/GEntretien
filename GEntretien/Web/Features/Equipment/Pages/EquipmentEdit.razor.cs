using GEntretien.Domain.Entities;
using GEntretien.Domain.Interfaces;
using Microsoft.AspNetCore.Components;

namespace GEntretien.Web.Features.Equipment.Pages;

public partial class EquipmentEdit
{
    [Inject]
    private IEquipmentRepository EquipmentRepository { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Parameter]
    public int Id { get; set; }

    private Domain.Entities.Equipment model = new();

    protected override async Task OnInitializedAsync()
    {
        if (Id != 0)
        {
            var e = await EquipmentRepository.GetByIdAsync(Id);
            if (e != null)
            {
                model = e;
            }
        }
    }

    private async Task HandleValidSubmit()
    {
        if (Id == 0)
        {
            await EquipmentRepository.AddAsync(model);
        }
        else
        {
            await EquipmentRepository.UpdateAsync(model);
        }

        NavigationManager.NavigateTo("/equipment");
    }

    private void Cancel()
    {
        NavigationManager.NavigateTo("/equipment");
    }
}
