using Microsoft.EntityFrameworkCore;
using Tracker.Admin.Web.Models;

namespace Tracker.Admin.Web.Data
{
    public class TrackerDbContext : DbContext
    {
        public TrackerDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            ConfigureEntities.ConfigurePerson(builder);
            ConfigureEntities.ConfigureCard(builder);
            ConfigureEntities.ConfigurePersonCard(builder);
            ConfigureEntities.ConfigureLocation(builder);
            ConfigureEntities.ConfigureDevice(builder);
            ConfigureEntities.ConfigureMovement(builder);
        }

        public DbSet<Person> Person { get; set; }
        public DbSet<Card> Card { get; set; }
        public DbSet<PersonCard> PersonCard { get; set; }
        public DbSet<Location> Location { get; set; }
        public DbSet<Device> Device { get; set; }
        public DbSet<Movement> Movement { get; set; }
    }
}