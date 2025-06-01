using FiapCloudGames.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FiapCloudGames.Infrastructure.Repository
{
    public class ApplicationDbContext : DbContext
    {
        private readonly string _connectionString;

        public ApplicationDbContext()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json")
                    .Build();

            _connectionString = configuration.GetConnectionString("ConnectionString");
        }

        public ApplicationDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DbSet<Jogo> Jogo { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<UsuarioJogoPropriedade> UsuarioJogo { get; set; }
        public DbSet<LogEntity> Logs { get; set; }
        public DbSet<Promocao> Promocoes { get; set; }
        public DbSet<JogosPromocoes> JogosPromocoes { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_connectionString);
                optionsBuilder.UseLazyLoadingProxies();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly); // Evita repetição de linhas

            base.OnModelCreating(modelBuilder);
                        
            modelBuilder.Entity<JogosPromocoes>()
                .HasOne(jp => jp.Usuario)
                .WithMany(u => u.JogosPromocoes)
                .HasForeignKey(jp => jp.UsuarioId)
                .OnDelete(DeleteBehavior.NoAction);
                        
            modelBuilder.Entity<Promocao>()
                .HasOne(p => p.Usuario)
                .WithMany(u => u.Promocoes)
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.NoAction);
                        
            modelBuilder.Entity<Jogo>()
                .HasOne(j => j.Usuario)
                .WithMany(u => u.Jogos)
                .HasForeignKey(j => j.UsuarioId)
                .OnDelete(DeleteBehavior.NoAction);
        }

        public override int SaveChanges()
        {
            foreach(var entry in ChangeTracker.Entries<EntityBase>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.DataCriacao = DateTime.Now;
                }                    
            }

            return base.SaveChanges();
        }

    }
}
