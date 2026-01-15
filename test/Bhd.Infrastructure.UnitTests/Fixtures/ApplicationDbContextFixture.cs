using Bhd.Infrastructure.Persistance.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Bhd.Infrastructure.UnitTests.Fixtures;

public class ApplicationDbContextFixture : IDisposable
{
    public ApplicationDbContext Context { get; private set; }

    public ApplicationDbContextFixture()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        Context = new ApplicationDbContext(options);
        Context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();
    }

    public void ClearDatabase()
    {
        Context.Documents.RemoveRange(Context.Documents);
        Context.Users.RemoveRange(Context.Users);
        Context.SaveChanges();
    }
}
