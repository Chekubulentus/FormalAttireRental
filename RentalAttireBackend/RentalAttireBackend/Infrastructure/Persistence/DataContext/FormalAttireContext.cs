using Microsoft.EntityFrameworkCore;
using RentalAttireBackend.Domain.Entities;

namespace RentalAttireBackend.Infrastructure.Persistence.DataContext
{
    public class FormalAttireContext : DbContext
    {
        public FormalAttireContext(DbContextOptions<FormalAttireContext> options) : base(options) { }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>(e =>
            {

            });
        }
    }
}
