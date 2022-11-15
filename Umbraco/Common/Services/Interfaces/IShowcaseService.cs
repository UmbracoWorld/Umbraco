using Umbraco.Common.Models.Dtos;

namespace Umbraco.Common.Services;

public interface IShowcaseService
{
    Task<PaginatedList<Showcase>> GetAllShowcases(int pageSize = 10, int currentPage = 1);
    Task<Showcase?> GetShowcaseById(string id);
}