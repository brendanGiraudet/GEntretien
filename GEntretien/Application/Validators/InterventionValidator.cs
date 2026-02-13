using FluentValidation;
using GEntretien.Domain.Entities;

namespace GEntretien.Application.Validators
{
    public class InterventionValidator : AbstractValidator<Intervention>
    {
        public InterventionValidator()
        {
            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("La date est requise");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("La description est requise")
                .MaximumLength(1000).WithMessage("La description est trop longue");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Le statut est requis")
                .Must(x => new[] { "Planifie", "En cours", "Terminee", "Annulee" }.Contains(x))
                .WithMessage("Le statut n'est pas valide");

            RuleFor(x => x.EquipmentId)
                .GreaterThan(0).WithMessage("L'équipement est requis");
        }
    }
}
