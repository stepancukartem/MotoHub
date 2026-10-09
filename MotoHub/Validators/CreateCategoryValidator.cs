
using FluentValidation;
using MotoHub.DTOs;

namespace MotoHub.Validators
{
    public class CreateCategoryValidator
        : AbstractValidator<CreateCategoryDto>
    {
        public CreateCategoryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);
        }
    }
}
