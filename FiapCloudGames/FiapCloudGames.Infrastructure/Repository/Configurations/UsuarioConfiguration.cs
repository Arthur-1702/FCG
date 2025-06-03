using FiapCloudGames.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiapCloudGames.Infrastructure.Repository.Configurations
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("Usuario");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).HasColumnType("INT").UseIdentityColumn();
            builder.Property(p => p.Nome).HasColumnType("VARCHAR(100)").IsRequired();
            builder.Property(p => p.Email).HasColumnType("VARCHAR(100)").IsRequired();
            builder.Property(p => p.Senha).HasColumnType("VARCHAR(100)").IsRequired();
            builder.Property(p => p.NivelAcesso).HasColumnType("VARCHAR(100)").IsRequired();
            builder.Property(p => p.Saldo).HasColumnType("DECIMAL(10,2)").IsRequired();
            builder.Property(p => p.DataCriacao).HasColumnType("DATETIME").IsRequired();
        }
    }
}
