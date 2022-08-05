using FluentValidation.TestHelper;
using Service.Showcase.Application.Showcase.Queries.GetShowcaseById;
using Xunit;

namespace Service.Showcase.Tests.Showcases.Queries.GetShowcaseById;

public class GetAuthorByIdQueryValidatorTests
{
    private static readonly GetShowcaseByIdValidator Validator = new();

    [Fact]
    public void Validator_ShouldHaveValidationErrorFor_IdNull()
    {
        // Arrange
        var query = new GetShowcaseByIdQuery();

        // Act
        var result = Validator.TestValidate(query);

        // Assert
        _ = result.ShouldHaveValidationErrorFor(query => query.Id);
    }

    [Fact]
    public void Validator_ShouldHaveValidationErrorFor_IdEmpty()
    {
        // Arrange
        var query = new GetShowcaseByIdQuery { Id = Guid.Empty };

        // Act
        var result = Validator.TestValidate(query);

        // Assert
        _ = result.ShouldHaveValidationErrorFor(query => query.Id);
    }

    [Fact]
    public void Validator_ShouldNotHaveValidationErrorFor_Id()
    {
        // Arrange
        var query = new GetShowcaseByIdQuery { Id = Guid.NewGuid() };

        // Act
        var result = Validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(query => query.Id);
    }
}