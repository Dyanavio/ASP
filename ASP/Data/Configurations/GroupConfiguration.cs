using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ASP.Data.Entities;
using System.Reflection.Emit;

namespace ASP.Data.Configurations
{
    public class GroupConfiguration : IEntityTypeConfiguration<Entities.ProductGroup>
    {
        public void Configure(EntityTypeBuilder<Entities.ProductGroup> builder)
        {
            builder.HasIndex(p => p.Slug).IsUnique();
            builder.HasOne(group => group.ParentGroup).WithMany().HasForeignKey(group => group.ParentId);
            builder.HasMany(group => group.Products).WithOne(product => product.Group).HasForeignKey(product => product.GroupId);
        }
    }
}
