using NSubstitute;
using Service.Showcase.Application.Showcase;
using Service.Showcase.Application.Showcase.Queries.GetShowcases;
using Shouldly;
using Xunit;

namespace Service.Showcase.Tests.Showcases.Queries.GetAuthors;

public class GetAuthorsHandlerTests
{
    [Fact]
    public async Task Handle_ShouldPassThrough_Query()
    {
        // Arrange
        var query = new GetShowcasesQuery();

        var context = Substitute.For<IShowcaseRepository>();
        var handler = new GetShowcasesHandler(context);
        var token = new CancellationTokenSource().Token;

        _ = context.GetShowcases(token).Returns(new List<Application.Showcase.Entities.Showcase>
        {
            new ()
            {
                Id = Guid.Empty,
                Title = "Title",
                Summary = "Summary",
                Description = "Description",
            }
        });

        // Act
        var result = await handler.Handle(query, token);

        // Assert
        _ = await context.Received(1).GetShowcases(token);

        result.ShouldNotBeEmpty();
        result.Count.ShouldBe(1);
        result[0].Id.ShouldBe(Guid.Empty);
    }
}
