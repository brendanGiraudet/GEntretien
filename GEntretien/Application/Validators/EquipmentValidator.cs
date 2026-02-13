using FluentValidation;
using GEntretien.Domain.Entities;

namespace GEntretien.Application.Validators
{
    public class EquipmentValidator : AbstractValidator<Equipment>
    {
        public EquipmentValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Le nom est requis")
                .MaximumLength(200).WithMessage("Le nom est trop long");

            RuleFor(x => x.SerialNumber)
                .MaximumLength(100).WithMessage("Le numéro de série est trop long");

            RuleFor(x => x.Location)
                .MaximumLength(150).WithMessage("La localisation est trop longue");

            RuleFor(x => x.PurchaseDate)
                .LessThanOrEqualTo(DateTime.Today).WithMessage("La date d'achat ne peut pas être dans le futur");
        }
    }
}
