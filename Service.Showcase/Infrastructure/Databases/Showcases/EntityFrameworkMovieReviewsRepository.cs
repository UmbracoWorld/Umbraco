namespace Service.Showcase.Infrastructure.Databases.Showcases;

using Application.Showcase;
using Application.Showcase.Entities;
using AutoMapper;
using Extensions;
using Microsoft.EntityFrameworkCore;
using SimpleDateTimeProvider;

internal class EntityFrameworkMovieReviewsRepository : IShowcaseRepository
{
    private readonly ShowcaseDbContext _context;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IMapper _mapper;

    public EntityFrameworkMovieReviewsRepository(ShowcaseDbContext context, IDateTimeProvider dateTimeProvider,
                                                 IMapper mapper)
    {
        _context = context;
        _dateTimeProvider = dateTimeProvider;
        _mapper = mapper;

        if (_context != null)
        {
            _ = _context.Database.EnsureDeleted();
            _ = _context.Database.EnsureCreated();
            _ = _context.AddData();
        }
    }
    

    public virtual async Task<List<Showcase>> GetShowcases(CancellationToken cancellationToken)
    {
        var authors = await _context.Showcases.ToListAsync(cancellationToken);

        return _mapper.Map<List<Showcase>>(authors);
    }

    public virtual async Task<Showcase> GetShowcaseById(Guid id, CancellationToken cancellationToken)
    {
        var author = await _context.Showcases
                         .Where(r => r.Id == id)
                         .FirstOrDefaultAsync(cancellationToken);

        return _mapper.Map<Showcase>(author);
    }

    public virtual async Task<bool> ShowcaseExists(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Showcases.AsNoTracking().AnyAsync(a => a.Id == id, cancellationToken);
    }
    

}
