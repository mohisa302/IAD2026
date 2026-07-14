using IAD2026.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IAD2026.Infrastructure.Persistence.Configurations;

public class DcimSnapshotConfiguration
    : IEntityTypeConfiguration<DcimSnapshot>
{
    public void Configure(EntityTypeBuilder<DcimSnapshot> builder)
    {
        builder.ToTable("DcimSnapshots");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.JsonBody)
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        builder.Property(x => x.DcimType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.CurrentDate)
            .HasColumnType("datetime2")
            .IsRequired();
    }
}