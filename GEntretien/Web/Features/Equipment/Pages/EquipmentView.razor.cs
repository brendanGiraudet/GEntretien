using GEntretien.Domain.Entities;
using GEntretien.Domain.Interfaces;
using Microsoft.AspNetCore.Components;

namespace GEntretien.Web.Features.Equipment.Pages;

public partial class EquipmentView
{
    [Parameter]
    public int EquipmentId { get; set; }

    [Inject]
    private IEquipmentRepository _equipmentRepository { get; set; } = default!;

    [Inject]
    private IInterventionRepository _interventionRepository { get; set; } = default!;

    private Domain.Entities.Equipment? _equipment;
    private List<Intervention>? _interventions;
    private bool _loadingError;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            _equipment = await _equipmentRepository.GetByIdAsync(EquipmentId);
            
            if (_equipment is not null)
            {
                _interventions = await _interventionRepository.ListByEquipmentAsync(EquipmentId);
            }
            else
            {
                _loadingError = true;
            }
        }
        catch
        {
            _loadingError = true;
        }
    }
}
