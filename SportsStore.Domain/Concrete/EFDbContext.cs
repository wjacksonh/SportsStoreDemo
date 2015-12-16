using System;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using System.Data.Entity;

namespace SportsStore.Domain.Concrete {
    public class EFDbContext : DbContext {

        public EFDbContext() : base("EFDbContextConnection") {

        }

        public DbSet<Product> Products { get; set; }
    }
}
