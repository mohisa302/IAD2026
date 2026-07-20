using IAD2026.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IAD2026.Infrastructure.Persistence.Configurations;

public class StorageServerPortConfiguration
    : IEntityTypeConfiguration<StorageServerPort>
{
    public void Configure(EntityTypeBuilder<StorageServerPort> builder)
    {
        builder.ToTable("Storage_ServerPort");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.HostName)
            .HasColumnType("nvarchar(100)")
            .IsRequired();


        builder.Property(x => x.DeviceId)
            .HasColumnType("nvarchar(50)")
            .IsRequired();

        
        builder.Property(x => x.HwAddress)
            .HasColumnType("nvarchar(20)")
            .IsRequired();
    }
}