using MediatR;

namespace Service.Showcase.Application.Showcase.Queries.GetShowcases;

public class GetShowcasesHandler : IRequestHandler<GetShowcasesQuery, List<Entities.Showcase>>
{
    private readonly IShowcaseRepository _repository;

    public GetShowcasesHandler(IShowcaseRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Entities.Showcase>> Handle(GetShowcasesQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetShowcases(cancellationToken);
    }
}
