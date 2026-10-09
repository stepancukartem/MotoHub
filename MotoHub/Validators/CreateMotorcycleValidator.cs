
using FluentValidation;
using MotoHub.DTOs;

namespace MotoHub.Validators
{
    public class CreateMotorcycleValidator
        : AbstractValidator<CreateMotorcycleDto>
    {
        public CreateMotorcycleValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Year)
                .InclusiveBetween(1900, 2100);

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.EngineVolume)
                .GreaterThan(0);

            RuleFor(x => x.Mileage)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.Description)
                .NotEmpty();

            RuleFor(x => x.BrandId)
                .GreaterThan(0);

            RuleFor(x => x.CategoryId)
                .GreaterThan(0);
        }
    }
}
