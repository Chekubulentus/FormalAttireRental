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
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }
        public DbSet<ReceivingBatch> ReceivingBatches { get; set; }
        public DbSet<ReceivingBatchItem> ReceivingBatchItems { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Customer Constraints
            modelBuilder.Entity<Customer>(e =>
            {
                e.HasKey(c => c.Id);
                e.HasIndex(c => c.CustomerCode).IsUnique();

                //User Relationship
                e.HasOne(c => c.User)
                .WithOne(u => u.Customer)
                .HasForeignKey<Customer>(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            });
            #endregion

            #region Employee Constraints
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

            #region Person Constraints
            modelBuilder.Entity<Person>(e =>
            {
                e.HasKey(p => p.Id);
                e.Property(p => p.PhoneNumber).HasMaxLength(11);
            });
            #endregion

            #region User Constraints
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

            #region Main Administrator Data

            // Person
            modelBuilder.Entity<Person>()
                .HasData(new Person
                {
                    Id = 2,

                    LastName = "Administrator",
                    FirstName = "Main",
                    MiddleName = "System",

                    Age = 30,
                    Gender = Gender.Others,
                    MaritalStatus = MaritalStatus.Single,

                    PhoneNumber = "09000000000",

                    Street = "System Street",
                    Barangay = "System Barangay",
                    City = "System City",
                    Province = "System Province",
                    PostalCode = "0000",

                    ProfileImagePath = null,

                    CreatedBy = "System",
                    CreatedAt = new DateTime(2026, 8, 20, 0, 0, 0, DateTimeKind.Utc),

                    UpdatedBy = string.Empty,
                    UpdatedAt = null,

                    ArchivedBy = string.Empty,
                    ArchivedAt = null,

                    RestoredBy = string.Empty,
                    RestoredAt = null,

                    IsActive = true,
                    IsDeleted = false,

                    EntityType = "Person"
                });


            // User
            modelBuilder.Entity<User>()
                .HasData(new User
                {
                    Id = 2,

                    Email = "admin",
                    HashedPassword = "admin",
                    RefreshToken = string.Empty,
                    RefreshTokenExpiryTime = null,

                    PersonId = 2,

                    IsGoogleAccount = false,

                    CreatedBy = "System",
                    CreatedAt = new DateTime(2026, 8, 20, 0, 0, 0, DateTimeKind.Utc),

                    UpdatedBy = string.Empty,
                    UpdatedAt = null,

                    ArchivedBy = string.Empty,
                    ArchivedAt = null,

                    RestoredBy = string.Empty,
                    RestoredAt = null,

                    IsActive = true,
                    IsDeleted = false,

                    EntityType = "User",
                });


            // Employee
            modelBuilder.Entity<Employee>()
                .HasData(new Employee
                {
                    Id = 1,

                    EmployeeCode = "EMP-000001",
                    Department = "Administration",
                    Salary = 0.00,

                    RoleId = 1,
                    UserId = 2,

                    CreatedBy = "System",
                    CreatedAt = new DateTime(2026, 8, 20, 0, 0, 0, DateTimeKind.Utc),

                    UpdatedBy = string.Empty,
                    UpdatedAt = null,

                    ArchivedBy = string.Empty,
                    ArchivedAt = null,

                    RestoredBy = string.Empty,
                    RestoredAt = null,

                    IsActive = true,
                    IsDeleted = false,

                    EntityType = "Employee"
                });

            #endregion

            #region AuditLog Constraints
            modelBuilder.Entity<AuditLog>(e =>
            {
                e.HasKey(a => a.Id);
                e.Property(a => a.EntityType).IsRequired().HasMaxLength(255);
                e.Property(a => a.ActionType).IsRequired();
                e.Property(a => a.ChangedBy).IsRequired();
                e.Property(a => a.ChangedAt).IsRequired();
            });
            #endregion

            #region Clothes Constraints
            modelBuilder.Entity<Clothe>(e =>
            {
                e.HasKey(c => c.Id);
                e.HasIndex(c => c.ClotheCode)
                .IsUnique();
                e.HasIndex(c => c.ClotheName)
                .IsUnique();

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

            #region Rental Constraints
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

            #region PurchaseOrder Constraints
            modelBuilder.Entity<PurchaseOrder>(e =>
            {
                e.HasKey(po => po.Id);
                e.HasIndex(po => po.PurchaseOrderCode);


                //PurchaseOrderItem Relationship
                e.HasMany(po => po.PurchaseOrderItems)
                .WithOne(poi => poi.PurchaseOrder)
                .HasForeignKey(poi => poi.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Restrict);

                //Supplier Relationship
                e.HasOne(po => po.Supplier)
                .WithOne(s => s.PurchaseOrder)
                .HasForeignKey<PurchaseOrder>(po => po.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

                //Employee Relationship
                e.HasOne(po => po.Employee)
                .WithMany(e => e.PurchaseOrders)
                .HasForeignKey(po => po.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PurchaseOrderItem>(e =>
            {
                e.HasKey(poi => poi.Id);

                //Clothe Relationship
                e.HasOne(poi => poi.Clothe)
                .WithMany(c => c.PurchaseOrderItems)
                .HasForeignKey(poi => poi.ClotheId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ReceivingBatch>(e =>
            {
                e.HasKey(r => r.Id);
                e.HasIndex(r => r.ReceivingBatchCode);

                //PurchaseOrder Relationship
                e.HasOne(r => r.PurchaseOrder)
                .WithOne()
                .HasForeignKey<ReceivingBatch>(r => r.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Restrict);

                //Employee Relationship
                e.HasOne(r => r.Employee)
                .WithMany(e => e.ReceivingBatches)
                .HasForeignKey(r => r.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ReceivingBatchItem>(e =>
            {
                e.HasKey(ri => ri.Id);

                //PurchaseOrderItem Relationship
                e.HasOne(ri => ri.PurchaseOrderItem)
                .WithMany(poi => poi.ReceivingBatchItems)
                .HasForeignKey(ri => ri.PurchaseOrderItemId)
                .OnDelete(DeleteBehavior.Restrict);
            });
            #endregion
        }
    }
}
