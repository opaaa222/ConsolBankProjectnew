using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using BankProject.Domain.Models;
using Microsoft.Identity.Client;
namespace BankProject.Infrastructure.Data
{
    public class BankDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<LoginEntry> LoginEntries { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=BankDb;Trusted_Connection=True;TrustServerCertificate=True;");

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(a =>
            {
                a.HasKey(u => u.Id);
                a.Property(u => u.Id).ValueGeneratedNever();
                a.Property(u => u.UserName).IsRequired().HasMaxLength(50);
                a.Property(u => u.Email).IsRequired().HasMaxLength(100);
                a.Property(u => u.Password).IsRequired().HasMaxLength(100);
                a.Property(u => u.Role).IsRequired().HasMaxLength(50);
                a.HasDiscriminator<string>("UserType")
                 .HasValue<Admin>("Admin")
                 .HasValue<Client>("Client");


            });

            modelBuilder.Entity<Client>(a =>
            {
                a.Property(c => c.Balance).HasPrecision(18, 2);
                a.Property(c => c.LoadStatus).IsRequired().HasMaxLength(50);
                a.Property(c => c.LoanAmount).HasPrecision(18, 2);
                a.Property(c => c.Salary).HasPrecision(18, 2);
                a.Property(c => c.IsBlocked).IsRequired();






            });


            modelBuilder.Entity<LoginEntry>(h =>
            {   
                

                h.Property(l => l.UserName).IsRequired().HasMaxLength(50);
                h.Property(l => l.IpAddress).IsRequired().HasMaxLength(50);
                h.Property(l => l.LoginTime).IsRequired();





            }
            );

            modelBuilder.Entity<Transaction>(t =>
            {
                t.Property(tr => tr.ClientId).IsRequired();
                t.Property(tr => tr.Type).IsRequired().HasMaxLength(50);
                t.Property(tr => tr.Amount).HasPrecision(18, 2);
                t.Property(tr => tr.BalanceAfter).HasPrecision(18, 2);
                t.Property(tr => tr.Date).IsRequired();




            });


        }








     }
}
