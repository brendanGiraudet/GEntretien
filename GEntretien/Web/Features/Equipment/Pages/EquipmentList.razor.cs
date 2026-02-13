using GEntretien.Domain.Entities;
using GEntretien.Domain.Interfaces;
using Microsoft.AspNetCore.Components;

namespace GEntretien.Web.Features.Equipment.Pages;

public partial class EquipmentList
{
    [Inject]
    private IEquipmentRepository _equipmentRepository { get; set; } = default!;

    [Inject]
    private NavigationManager _navigationManager { get; set; } = default!;

    private List<Domain.Entities.Equipment>? _equipments;

    protected override async Task OnInitializedAsync()
    {
        await Load();
    }

    private async Task Load()
    {
        _equipments = await _equipmentRepository.ListAsync();
    }

    private void CreateNew()
    {
        _navigationManager.NavigateTo("/equipment/edit/0");
    }

    private async Task Delete(int id)
    {
        await _equipmentRepository.DeleteAsync(id);
        await Load();
        StateHasChanged();
    }
}
