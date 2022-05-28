using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Users.Domain.DataAccess;
using Users.Domain.Entities;

namespace Users.EntityFramework.Relations
{
    public class ApplicationRoleEntityConfiguration : IEntityTypeConfiguration<ApplicationRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationRole> builder)
        {
            builder.ToTable(nameof(ApplicationRole), DatabaseConstants.Schema);

            builder.HasData(new ApplicationRole
            {
                Id = ApplicationRole.Admin.Id,
                Name = ApplicationRole.Admin.Name,
                ConcurrencyStamp = "BD7DF7A2-5F2B-45EC-B197-BBC28A09DB69",
                NormalizedName = ApplicationRole.Admin.NormalizedName
            });

        }
    }
}
