using Microsoft.EntityFrameworkCore;
using Rentify.Backend.Core.Domain.Entities;
using Rentify.Backend.Infraestructure.Persistence.EntityConfiguration;

namespace Rentify.Backend.Infraestructure.Persistence.Context
{
    public class RentifyContext : DbContext
    {
        public RentifyContext(DbContextOptions<RentifyContext> options) : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new RentCarEntityConfiguration());
        }

        public DbSet<RentCar> RentCars { get; set; }
    }
}