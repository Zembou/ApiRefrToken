using ApiRefr.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiRefr.Context
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions dbContextOptions)
            : base(dbContextOptions)
        {
        }

        public DbSet<LoginModel>? LoginModels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LoginModel>().HasData(new LoginModel
            {
                Id = 1,
                UserName = "ncs56",
                Password = "ncs56mdp"
            });
        }
    }
}
