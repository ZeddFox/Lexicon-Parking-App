using Microsoft.EntityFrameworkCore;

namespace Lexicon_Parking_App
{
    public class PersistanceContext : DbContext
    {
        public PersistanceContext(DbContextOptions<PersistanceContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Period> Periods { get; set; }
    }
}
