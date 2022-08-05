namespace Service.Showcase.Application.Showcase;

using System.Threading.Tasks;
using Entities;

public interface IShowcaseRepository
{
    Task<List<Showcase>> GetShowcases(CancellationToken cancellationToken);
    Task<Showcase> GetShowcaseById(Guid id, CancellationToken cancellationToken);
    Task<bool> ShowcaseExists(Guid id, CancellationToken cancellationToken);
}
