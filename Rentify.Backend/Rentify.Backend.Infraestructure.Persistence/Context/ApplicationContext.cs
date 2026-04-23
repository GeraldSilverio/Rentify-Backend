using Microsoft.EntityFrameworkCore;

namespace Rentify.Backend.Infraestructure.Persistence.Context
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {

        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}