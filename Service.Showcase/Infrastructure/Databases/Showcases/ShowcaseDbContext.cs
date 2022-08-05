namespace Service.Showcase.Infrastructure.Databases.Showcases;

using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Models;

internal class ShowcaseDbContext : DbContext
{
    public ShowcaseDbContext(DbContextOptions<ShowcaseDbContext> options) : base(options)
    {
    }

    public DbSet<Showcase> Showcases { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        _ = modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
