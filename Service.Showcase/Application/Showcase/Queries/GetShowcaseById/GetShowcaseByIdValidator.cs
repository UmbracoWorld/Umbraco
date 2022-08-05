namespace Service.Showcase.Application.Showcase.Queries.GetShowcaseById;

using FluentValidation;

public class GetShowcaseByIdValidator : AbstractValidator<GetShowcaseByIdQuery>
{
    public GetShowcaseByIdValidator()
    {
        _ = this.RuleFor(r => r.Id)
            .NotNull()
            .NotEqual(Guid.Empty)
            .WithMessage("A showcase Id was not supplied.");
    }
}
