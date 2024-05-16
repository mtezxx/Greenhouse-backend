using EfcDataAccess;
using Microsoft.EntityFrameworkCore;

namespace Tests.Utils;

public class DbTestBase
{
    public EfcContext DbContext { get; private set; }
    
    public virtual void TestInit()
    {
        // configure an in memory database
        DbContext = new DbContextInMemory();
        // this ensures that the database is created in the memory and it's ready to use
        DbContext.Database.EnsureCreated();
    }


    private class DbContextInMemory : EfcContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());
        }
    }
}