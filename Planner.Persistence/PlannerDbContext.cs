using Microsoft.EntityFrameworkCore;
using Planner.Entities.Domain;

namespace Planner.Persistence
{
	public class PlannerDbContext : DbContext, IDisposable
    {
		public PlannerDbContext(DbContextOptions<PlannerDbContext> options) : base(options)
		{
		}

		public virtual DbSet<Sport> Sports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sport>(entity =>
            {
                entity.HasKey(x => x.Id);
            });
        }
    }
}

