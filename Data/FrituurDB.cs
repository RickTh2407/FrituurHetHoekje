using FrituurHetHoekje.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FrituurHetHoekje.Data
{
    public class FrituurDB : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            string connection = @"Data Source=asusdrg4n\sqldrg4n;Initial Catalog=FrituurDB;Integrated Security=True;Trust Server Certificate=True";
            optionsBuilder.UseSqlServer(connection);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //specify Order
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderProducts)
                .WithOne(o => o.Order)
                .HasForeignKey(o => o.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            //specify Account
            modelBuilder.Entity<Account>()
                .Property(v => v.FirstName)
                .HasMaxLength(30);

            //specify Product
            modelBuilder.Entity<Product>()
                .HasMany(p => p.OrderProducts)
                .WithOne(op => op.Product)
                .HasForeignKey(op => op.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            //specify OrderProduct
            modelBuilder.Entity<OrderProduct>()
                .HasKey(op => new { op.OrderId, op.ProductId });

            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.Order)
                .WithMany(o => o.OrderProducts)
                .HasForeignKey(op => op.OrderId);

            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.Product)
                .WithMany(p => p.OrderProducts)
                .HasForeignKey(op => op.ProductId);

            //data seed Order
            Order orderEntity = new Order()
            {
                Id = 1,
                OrderNr = 123456,
                //TotalPrice = 00.00,
                Date = DateTime.Now,
                AccountId = 1
            };
            modelBuilder.Entity<Order>()
                .HasData(orderEntity);

            //data seed Account
            Account accountEntity = new Account()
            {
                Id = 1,
                FirstName = "Test",
                LastName = "Tester",
                Phone = "0612345678",
                Email = "Test@gmail.com",
                Password = "TestPassword123",
                Staff = true,
                Owner = true,
                Points = 0
            };
            modelBuilder.Entity<Account>()
                .HasData(accountEntity);

            //data seed Product
            Product productEntity = new Product()
            {
                Id = 1,
                Name = "Test",
                Price = 11.11M,
                Photo = "C:\\Users\\rickt\\source\\repos\\FrituurHetHoekje\\Media\\Test.jpg",
            };
            modelBuilder.Entity<Product>()
                .HasData(productEntity);
        }
    }
}
