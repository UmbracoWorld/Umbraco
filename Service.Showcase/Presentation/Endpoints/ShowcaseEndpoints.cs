using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using MediatR;
using Service.Showcase.Application.Showcase.Queries.GetShowcaseById;
using Service.Showcase.Application.Showcase.Queries.GetShowcases;
using Service.Showcase.Presentation.Errors;
using Swashbuckle.AspNetCore.Annotations;

namespace Service.Showcase.Presentation.Endpoints;

[ExcludeFromCodeCoverage]
public static class ServiceEndpoints
{
    public static WebApplication MapEndpoints(this WebApplication app)
    {

        _ = app.MapGet("/api/healthcheck", () => "👍")
            .WithTags("Showcase")
            .WithMetadata(new SwaggerOperationAttribute("Simple 200 response to indicate api is running"))
            .Produces(200);
        
        _ = app.MapGet("/api/showcase", async (IMediator mediator) => Results.Ok(await mediator.Send(new GetShowcasesQuery())))
            .WithTags("Showcase")
            .WithMetadata(new SwaggerOperationAttribute("Lookup all showcases"))
            .Produces<List<Application.Showcase.Entities.Showcase>>(StatusCodes.Status200OK, contentType: MediaTypeNames.Application.Json)
            .Produces<ApiError>(StatusCodes.Status500InternalServerError, contentType: MediaTypeNames.Application.Json);

        _ = app.MapGet(
                "/api/showcases/{id:guid}",
                async (IMediator mediator, Guid id) =>
                    Results.Ok(await mediator.Send(new GetShowcaseByIdQuery { Id = id })))
            .WithTags("Showcase")
            .WithMetadata(new SwaggerOperationAttribute("Lookup a showcase by their Id"))
            .Produces<Application.Showcase.Entities.Showcase>(StatusCodes.Status200OK, contentType: MediaTypeNames.Application.Json)
            .Produces<ApiError>(StatusCodes.Status400BadRequest, contentType: MediaTypeNames.Application.Json)
            .Produces<ApiError>(StatusCodes.Status404NotFound, contentType: MediaTypeNames.Application.Json)
            .Produces<ApiError>(StatusCodes.Status500InternalServerError, contentType: MediaTypeNames.Application.Json);

        return app;
    }
}