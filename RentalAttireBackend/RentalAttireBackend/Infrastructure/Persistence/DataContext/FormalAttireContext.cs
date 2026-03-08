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
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Clothe> Clothes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<RentalItem> RentalItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Customer
            modelBuilder.Entity<Customer>(e =>
            {
                e.HasKey(c => c.Id);
                e.HasIndex(c => c.CustomerCode);

                //User Relationship
                e.HasOne(c => c.User)
                .WithOne(u => u.Customer)
                .HasForeignKey<Customer>(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            });
            #endregion

            #region Employee
            modelBuilder.Entity<Employee>(e =>
            {
                e.HasKey(e => e.Id);
                e.HasIndex(e => e.EmployeeCode);

                //Role Relationship
                e.HasOne(e => e.Role)
                .WithMany(r => r.Employees)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.SetNull);

                //User Relationship
                e.HasOne(e => e.User)
                .WithOne(u => u.Employee)
                .HasForeignKey<Employee>(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            });
            #endregion

            #region Person
            modelBuilder.Entity<Person>(e =>
            {
                e.HasKey(p => p.Id);
                e.Property(p => p.PhoneNumber).IsRequired().HasMaxLength(11);
            });
            #endregion

            #region User
            modelBuilder.Entity<User>(e =>
            {
                e.HasKey(u => u.Id);
                e.HasIndex(u => u.Email);


                //Person Relationship
                e.HasOne(u => u.Person)
                .WithOne(p => p.User)
                .HasForeignKey<User>(u => u.PersonId)
                .OnDelete(DeleteBehavior.Restrict);
            });
            #endregion

            #region Role Initial Data
            modelBuilder.Entity<Role>()
                .HasData(new Role
                {
                    Id = 1,
                    CreatedBy = "Papat",
                    CreatedAt = DateTime.SpecifyKind(new DateTime(2025, 1, 25), DateTimeKind.Utc),
                    IsActive = true,
                    IsDeleted = false,
                    EntityType = "Role",
                    RolePosition = RolePosition.Administrator,
                });
            modelBuilder.Entity<Role>()
                .HasData(new Role
                {
                    Id = 2,
                    CreatedBy = "Papat",
                    CreatedAt = DateTime.SpecifyKind(new DateTime(2025, 1, 25), DateTimeKind.Utc),
                    IsActive = true,
                    IsDeleted = false,
                    EntityType = "Role",
                    RolePosition = RolePosition.ClothesManager,
                });
            modelBuilder.Entity<Role>()
                .HasData(new Role
                {
                    Id = 3,
                    CreatedBy = "Papat",
                    CreatedAt = DateTime.SpecifyKind(new DateTime(2025, 1, 25), DateTimeKind.Utc),
                    IsActive = true,
                    IsDeleted = false,
                    EntityType = "Role",
                    RolePosition = RolePosition.Cashier,
                });

            #endregion

            #region Category Initial Data 
            modelBuilder.Entity<Category>()
                .HasData(new Category
                {
                    Id = 1,
                    CategoryCode = "C-0001",
                    CategoryName = "Gown",
                    Description = "Used for formal parties.",
                    IsActive = true,
                    IsDeleted = false,
                    EntityType = "Category",
                    CreatedBy = "Papat",
                    CreatedAt = DateTime.SpecifyKind(new DateTime(2025, 1, 25), DateTimeKind.Utc)
                });
            modelBuilder.Entity<Category>()
                .HasData(new Category
                {
                    Id = 2,
                    CategoryCode = "C-0002",
                    CategoryName = "Tuxedo",
                    Description = "Used for formal parties.",
                    IsActive = true,
                    IsDeleted = false,
                    EntityType = "Category",
                    CreatedBy = "Papat",
                    CreatedAt = DateTime.SpecifyKind(new DateTime(2025, 1, 25), DateTimeKind.Utc)
                });
            modelBuilder.Entity<Category>()
                .HasData(new Category
                {
                    Id = 3,
                    CategoryCode = "C-0003",
                    CategoryName = "Barong",
                    Description = "Used for  being dead.",
                    IsActive = true,
                    IsDeleted = false,
                    EntityType = "Category",
                    CreatedBy = "Papat",
                    CreatedAt = DateTime.SpecifyKind(new DateTime(2025, 1, 25), DateTimeKind.Utc)
                });
            #endregion

            #region AuditLog
            modelBuilder.Entity<AuditLog>(e =>
            {
                e.HasKey(a => a.Id);
                e.Property(a => a.EntityType).IsRequired().HasMaxLength(255);
                e.Property(a => a.ActionType).IsRequired();
                e.Property(a => a.ChangedBy).IsRequired();
                e.Property(a => a.ChangedAt).IsRequired();
            });
            #endregion

            #region Clothes
            modelBuilder.Entity<Clothe>(e =>
            {
                e.HasKey(c => c.Id);
                e.HasIndex(c => c.ClotheCode);

                e.HasOne(c => c.Category)
                .WithMany(c => c.Clothes)
                .HasForeignKey(c => c.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

                e.HasMany(c => c.RentalItems)
                .WithOne(ri => ri.Clothe)
                .HasForeignKey(ri => ri.ClotheId)
                .OnDelete(DeleteBehavior.Restrict);
            });
            #endregion

            #region Rental  
            modelBuilder.Entity<Rental>(e =>
            {
                e.HasKey(r => r.Id);
                e.HasIndex(r => r.RentalCode);

                e.HasMany(r => r.RentalItems)
                .WithOne(ri => ri.Rental)
                .HasForeignKey(ri => ri.RentalId)
                .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(r => r.Customer)
                .WithMany(c => c.Rentals)
                .HasForeignKey(r => r.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
            });
            #endregion
        }
    }
}
