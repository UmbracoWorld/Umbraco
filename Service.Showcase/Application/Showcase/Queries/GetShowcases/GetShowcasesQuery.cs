using MediatR;

namespace Service.Showcase.Application.Showcase.Queries.GetShowcases;

public class GetShowcasesQuery : IRequest<List<Entities.Showcase>>
{
}
