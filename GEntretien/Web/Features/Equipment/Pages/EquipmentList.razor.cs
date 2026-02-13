using GEntretien.Domain.Entities;
using GEntretien.Domain.Interfaces;
using Microsoft.AspNetCore.Components;

namespace GEntretien.Web.Features.Equipment.Pages;

public partial class EquipmentList
{
    [Inject]
    private IEquipmentRepository EquipmentRepository { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    private List<Domain.Entities.Equipment>? equipments;

    protected override async Task OnInitializedAsync()
    {
        await Load();
    }

    private async Task Load()
    {
        equipments = await EquipmentRepository.ListAsync();
    }

    private void CreateNew()
    {
        NavigationManager.NavigateTo("/equipment/edit/0");
    }

    private async Task Delete(int id)
    {
        await EquipmentRepository.DeleteAsync(id);
        await Load();
        StateHasChanged();
    }
}
