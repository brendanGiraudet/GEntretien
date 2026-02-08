using GEntretien.Application.Validators;
using GEntretien.Domain.Entities;
using Xunit;

namespace GEntretien.Tests.Validators
{
    public class EquipmentValidatorTests
    {
        private readonly EquipmentValidator _validator = new EquipmentValidator();

        [Fact]
        public void Should_have_error_when_name_is_empty()
        {
            var model = new Equipment { Name = "" };
            var result = _validator.Validate(model);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Name");
        }

        [Fact]
        public void Should_not_have_error_when_name_is_valid()
        {
            var model = new Equipment { Name = "Printer" };
            var result = _validator.Validate(model);
            Assert.True(result.IsValid);
        }
    }
}
