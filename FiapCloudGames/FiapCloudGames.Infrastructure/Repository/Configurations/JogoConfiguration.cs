using FiapCloudGames.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiapCloudGames.Infrastructure.Repository.Configurations
{
    public class JogoConfiguration : IEntityTypeConfiguration<Jogo>
    {
        public void Configure(EntityTypeBuilder<Jogo> builder)
        {
            builder.ToTable("Jogos");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).HasColumnType("INT").UseIdentityColumn();
            builder.Property(p => p.Nome).HasColumnType("VARCHAR(100)").IsRequired();
            builder.Property(p => p.Descricao).HasColumnType("VARCHAR(200)").IsRequired();
            builder.Property(p => p.Preco).HasColumnType("DECIMAL(10,2)").IsRequired();            
            builder.Property(p => p.Genero).HasColumnType("VARCHAR(100)").IsRequired();
            builder.Property(p => p.DataCriacao).HasColumnType("DATETIME").IsRequired();

            builder.HasOne(p => p.Usuario)
                   .WithMany(u => u.Jogos)
                   .HasForeignKey(p => p.UsuarioId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
