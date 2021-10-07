using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CustomerManager.Models;

namespace CustomerManager.DBContexts
{
    public class CustomerManagerContext : DbContext
    {
        public CustomerManagerContext(DbContextOptions<CustomerManagerContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Card> Cards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Card>()
                .HasOne(card => card.Customer)
                .WithMany(customer => customer.Cards)
                .HasForeignKey(card => card.CustomerId);
        }
    }
}
