
using FluentValidation;
using MotoHub.DTOs;

namespace MotoHub.Validators
{
    public class CreateBrandValidator : AbstractValidator<CreateBrandDto>
    {
        public CreateBrandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);
        }
    }
}
