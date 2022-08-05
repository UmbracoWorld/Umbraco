using FluentValidation;
using MediatR;
using NSubstitute;
using Service.Showcase.Application.Common.Behaviours;
using Service.Showcase.Application.Showcase.Queries.GetShowcaseById;
using Shouldly;
using Xunit;

namespace Service.Showcase.Tests.Common.Behaviours;

public class ValidationBehaviourTests : BaseTest
{
    [Fact]
    public async void Handle_ShouldValidate_NoErrors()
    {
        // Arrange
        var query = new GetShowcaseByIdQuery { Id = Guid.NewGuid() };
        var validators = new List<IValidator<GetShowcaseByIdQuery>> { new GetShowcaseByIdValidator() };
        var handler = new ValidationBehaviour<GetShowcaseByIdQuery, Application.Showcase.Entities.Showcase>(validators);
        var token = new CancellationTokenSource().Token;
        var deletgate = Substitute.For<RequestHandlerDelegate<Application.Showcase.Entities.Showcase>>();

        _ = deletgate().Returns(GetNewShowcase());

        // Act
        var result = await handler.Handle(query, token, deletgate);

        // Assert
        _ = result.ShouldNotBeNull();
    }

    [Fact]
    public void Handle_ShouldValidate_Errors()
    {
        // Arrange
        var query = new GetShowcaseByIdQuery { Id = Guid.Empty };
        var validators = new List<IValidator<GetShowcaseByIdQuery>> { new GetShowcaseByIdValidator() };
        var handler = new ValidationBehaviour<GetShowcaseByIdQuery, Application.Showcase.Entities.Showcase>(validators);
        var token = new CancellationTokenSource().Token;
        var deletgate = Substitute.For<RequestHandlerDelegate<Application.Showcase.Entities.Showcase>>();

        // Act
        var exception = Should.Throw<ValidationException>(async () => await handler.Handle(query, token, deletgate));

        // Assert
        _ = exception.ShouldNotBeNull();
        _ = exception.ShouldBeOfType<ValidationException>();
        exception.Errors.ShouldNotBeEmpty();

        var errors = exception.Errors.ToList();

        errors.Count.ShouldBe(1);
        errors[0].PropertyName.ShouldBe("Id");
    }
}
