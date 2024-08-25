using EggProductionProject_MVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EggProductionProject_MVC.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
		//protected override void OnModelCreating(ModelBuilder modelBuilder)
		//{
		//	modelBuilder.Entity<CarrierAddress>()
		//		.Ignore(c => c.TrackReceiveSources);

		//	base.OnModelCreating(modelBuilder);
		//}

		//public DbSet<Member> Members { get; set; }
    }
}
