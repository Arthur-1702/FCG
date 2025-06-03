using FiapCloudGames.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiapCloudGames.Infrastructure.Repository.Configurations
{
    public class LogConfiguration : IEntityTypeConfiguration<LogEntity>
    {
        public void Configure(EntityTypeBuilder<LogEntity> builder)
        {
            builder.ToTable("Logs");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).HasColumnType("INT").UseIdentityColumn();
            builder.Property(p => p.Message).HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            builder.Property(p => p.MessageTemplate).HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            builder.Property(p => p.Level).HasColumnType("NVARCHAR(128)").IsRequired(false);
            builder.Property(p => p.TimeStamp).HasColumnType("DATETIME").IsRequired();
            builder.Property(p => p.Exception).HasColumnType("NVARCHAR(MAX)").IsRequired(false);
            builder.Property(p => p.Properties).HasColumnType("NVARCHAR(MAX)").IsRequired(false);
        }
    }
}
