using GEntretien.Domain.Entities;
using GEntretien.Domain.Interfaces;
using Microsoft.AspNetCore.Components;

namespace GEntretien.Web.Features.Equipment.Pages;

public partial class EquipmentInterventions
{
    [Inject]
    private IEquipmentRepository _equipmentRepository { get; set; } = default!;

    [Inject]
    private IInterventionRepository _interventionRepository { get; set; } = default!;

    [Inject]
    private NavigationManager _navigationManager { get; set; } = default!;

    [Parameter]
    public int EquipmentId { get; set; }

    private Domain.Entities.Equipment? _equipment;
    private List<Intervention>? _interventions;
    private Intervention _interventionForm = new() 
    {
        Date = DateTime.Today,
        Status = "Planifie"
    };
    private bool _isEditingIntervention;
    private static readonly string[] _statusOptions = { "Planifie", "En cours", "Terminee", "Annulee" };

    protected override async Task OnInitializedAsync()
    {
        _equipment = await _equipmentRepository.GetByIdAsync(EquipmentId);
        if (_equipment is null)
        {
            return;
        }

        await LoadInterventions();

        _interventionForm.EquipmentId = EquipmentId;
    }

    private async Task LoadInterventions()
    {
        _interventions = await _interventionRepository.ListByEquipmentAsync(EquipmentId);
    }

    private async Task SaveIntervention()
    {
        if (_isEditingIntervention)
        {
            await _interventionRepository.UpdateAsync(_interventionForm);
        }
        else
        {
            await _interventionRepository.AddAsync(_interventionForm);
        }

        await LoadInterventions();
        ResetInterventionForm();
        StateHasChanged();
    }

    private void EditIntervention(Intervention intervention)
    {
        _interventionForm = new Intervention
        {
            Id = intervention.Id,
            EquipmentId = intervention.EquipmentId,
            Date = intervention.Date,
            Status = intervention.Status,
            Description = intervention.Description
        };

        _isEditingIntervention = true;
    }

    private async Task DeleteIntervention(int id)
    {
        await _interventionRepository.DeleteAsync(id);
        await LoadInterventions();
        StateHasChanged();
    }

    private void CancelInterventionEdit()
    {
        ResetInterventionForm();
    }

    private void ResetInterventionForm()
    {
        _interventionForm = new Intervention
        {
            Date = DateTime.Today,
            Status = _statusOptions[0]
        };
        _isEditingIntervention = false;
    }
}
