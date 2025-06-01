using FiapCloudGames.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiapCloudGames.Infrastructure.Repository.Configurations
{
    public class UsuarioJogoConfiguration : IEntityTypeConfiguration<UsuarioJogoPropriedade>
    {
        public void Configure(EntityTypeBuilder<UsuarioJogoPropriedade> builder)
        {
            builder.ToTable("UsuarioJogo");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).HasColumnType("INT").UseIdentityColumn();
            builder.Property(p => p.UsuarioId).HasColumnType("INT").IsRequired();
            builder.Property(p => p.JogoId).HasColumnType("INT").IsRequired();
            builder.Property(p => p.ValorPago).HasColumnType("DECIMAL(10,2)").IsRequired();
            builder.Property(p => p.DataCriacao).HasColumnType("DATETIME").IsRequired();

            builder.HasOne(p => p.Usuario)
                   .WithMany(u => u.JogosObtidos)
                   .HasForeignKey(p => p.UsuarioId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Jogo)
                   .WithMany(j => j.Proprietarios)
                   .HasForeignKey(p => p.JogoId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Promocao)
                   .WithMany(pr => pr.UsuarioJogos)
                   .HasForeignKey(p => p.PromocaoId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
