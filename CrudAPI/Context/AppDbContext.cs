using CrudAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace CrudAPI.Context
{
    public class AppDbContext : DbContext
    {
        public DbSet<Tarea> Tareas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>(tb => {
                // Configuraciones de la entidad Usuario
                tb.HasKey(col => col.Id);
                tb.Property(col => col.Id).UseIdentityColumn().ValueGeneratedOnAdd();
                tb.Property(col => col.Nombre).IsRequired().HasMaxLength(100);
                tb.Property(col => col.Email).IsRequired().HasMaxLength(100);
                tb.HasIndex(col => col.Email).IsUnique();
                tb.ToTable("Usuario");
            });

            modelBuilder.Entity<Tarea>(tb => {
                // Configuraciones de la entidad Tarea
                tb.HasKey(col => col.Id);
                tb.Property(col => col.Id).UseIdentityColumn().ValueGeneratedOnAdd();
                tb.Property(col => col.Titulo).IsRequired().HasMaxLength(100);
                tb.Property(col => col.FechaCreacion).IsRequired();
                tb.Property(col => col.FechaVencimiento).IsRequired();
                tb.HasOne(col => col.Usuario)
                  .WithMany(u => u.Tareas)
                  .HasForeignKey(col => col.UsuarioId);
                tb.ToTable("Tarea");
            });
        }
    }
}
