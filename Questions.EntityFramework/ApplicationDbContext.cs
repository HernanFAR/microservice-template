using Microsoft.EntityFrameworkCore;
using Questions.Domain.Entities;

namespace Questions.EntityFramework
{
    public sealed class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Question> Questions => Set<Question>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
