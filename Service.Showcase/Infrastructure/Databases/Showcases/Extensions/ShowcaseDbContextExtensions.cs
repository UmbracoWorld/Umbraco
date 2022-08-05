namespace Service.Showcase.Infrastructure.Databases.Showcases.Extensions;

using System;
using Bogus;
using Models;

internal static class ShowcaseDbContextExtensions
{
    public static ShowcaseDbContext AddData(this ShowcaseDbContext context)
    {
        var authors = new Faker<Showcase>()
            .RuleFor(a => a.Id, _ => Guid.NewGuid())
            .RuleFor(a => a.Title, f => f.Commerce.ProductName())
            .RuleFor(a => a.Summary, f => f.Commerce.ProductDescription())
            .RuleFor(a => a.Description, f => f.Commerce.ProductDescription())
            .RuleFor(a => a.AuthorId, Guid.NewGuid())
            .RuleFor(a => a.DateCreated, f => f.Date.Past())
            .RuleFor(a => a.DateModified, f => f.Date.Past())
            .Generate(15);

        context.AddRange(authors);
        _ = context.SaveChanges();

        return context;
    }
}
