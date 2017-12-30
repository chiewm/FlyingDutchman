using System.Data.Entity;

namespace FlyingDutchman.Models
{
    public class MyShopDbContext:DbContext
    {
        public MyShopDbContext() : base("MyShop") { }
        public DbSet<User> Users { get; set; }
    }
}