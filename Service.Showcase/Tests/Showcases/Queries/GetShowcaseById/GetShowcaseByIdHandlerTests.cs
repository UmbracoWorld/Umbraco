using NSubstitute;
using Service.Showcase.Application.Common.Exceptions;
using Service.Showcase.Application.Showcase;
using Service.Showcase.Application.Showcase.Queries.GetShowcaseById;
using Shouldly;
using Xunit;

namespace Service.Showcase.Tests.Showcases.Queries.GetShowcaseById;

public class GetShowcaseByIdHandlerTests : BaseTest
{
    [Fact]
    public async Task Handle_ShouldPassThrough_Query()
    {
        // Arrange
        var query = new GetShowcaseByIdQuery { Id = Guid.Empty };

        var context = Substitute.For<IShowcaseRepository>();
        var handler = new GetShowcaseByIdHandler(context);
        var token = new CancellationTokenSource().Token;

        _ = context.GetShowcaseById(Arg.Any<Guid>(), token).Returns(GetNewShowcase());

        // Act
        var result = await handler.Handle(query, token);

        // Assert
        _ = await context.Received(1).GetShowcaseById(query.Id, token);

        _ = result.ShouldNotBeNull();
        result.Id.ShouldBe(Guid.Empty);
        result.Title.ShouldBe(ShowcaseTestConstants.Title);
        result.Description.ShouldBe(ShowcaseTestConstants.Description);
        result.Summary.ShouldBe(ShowcaseTestConstants.Summary);
        result.AuthorId.ShouldBe(Guid.Empty);
    }


    [Fact]
    public async Task Handle_ShouldThrowException_DoesNotExist()
    {
        // Arrange
        var query = new GetShowcaseByIdQuery { Id = Guid.Empty };

        var context = Substitute.For<IShowcaseRepository>();
        var handler = new GetShowcaseByIdHandler(context);
        var token = new CancellationTokenSource().Token;

        // Act
        var exception = Should.Throw<NotFoundException>(async () => await handler.Handle(query, token));

        // Assert
        _ = await context.Received(1).GetShowcaseById(query.Id, token);

        exception.Message.ShouldBe("The showcase with the supplied id was not found.");
    }
}
