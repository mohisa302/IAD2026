using IAD2026.Domain.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IAD2026.Persistence.Configurations;

public class DcimPhysicalViewConfiguration
    : IEntityTypeConfiguration<DcimPhysicalUnique>
{
    public void Configure(EntityTypeBuilder<DcimPhysicalUnique> builder)
    {
        builder.ToView("DCIM_Physical_Unique");
        builder.HasNoKey();
                
    }
}