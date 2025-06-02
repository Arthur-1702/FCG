using FiapCloudGames.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiapCloudGames.Infrastructure.Repository.Configurations
{
    public class PromocaoConfiguration : IEntityTypeConfiguration<Promocao>
    {
        public void Configure(EntityTypeBuilder<Promocao> builder)
        {
            builder.ToTable("Promocao");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).HasColumnType("INT").UseIdentityColumn();
            builder.Property(p => p.Descricao).HasColumnType("VARCHAR(100)").IsRequired();
            builder.Property(p => p.DataInicio).HasColumnType("DATETIME").IsRequired();
            builder.Property(p => p.DataFim).HasColumnType("DATETIME").IsRequired();
            builder.Property(p => p.Ativo).HasColumnType("BIT").IsRequired();
            builder.Property(p => p.DataCriacao).HasColumnType("DATETIME").IsRequired();

            builder.HasOne(p => p.Usuario)
                   .WithMany(u => u.Promocoes)
                   .HasForeignKey(p => p.UsuarioId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
