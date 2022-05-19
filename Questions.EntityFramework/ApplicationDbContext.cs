using Microsoft.EntityFrameworkCore;
using Questions.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

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
