using FiapCloudGames.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiapCloudGames.Infrastructure.Repository.Configurations
{
    public class JogosPromocoesConfiguration : IEntityTypeConfiguration<JogosPromocoes>
    {
        public void Configure(EntityTypeBuilder<JogosPromocoes> builder)
        {
            builder.ToTable("JogosPromocoes");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).HasColumnType("INT").UseIdentityColumn();
            builder.Property(p => p.JogoId).HasColumnType("INT").IsRequired();
            builder.Property(p => p.PromocaoId).HasColumnType("INT").IsRequired();
            builder.Property(p => p.UsuarioId).HasColumnType("INT").IsRequired();
            builder.Property(p => p.Desconto).HasColumnType("DECIMAL(18,2)").IsRequired();
            builder.Property(p => p.DataCriacao).HasColumnType("DATETIME").IsRequired();

            builder.HasOne(p => p.Usuario)
                   .WithMany(u => u.JogosPromocoes)
                   .HasForeignKey(p => p.UsuarioId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(p => p.Jogo)
                   .WithMany(j => j.Promocoes)
                   .HasForeignKey(p => p.JogoId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Promocao)
                   .WithMany(pr => pr.JogosPromocoes)
                   .HasForeignKey(p => p.PromocaoId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
