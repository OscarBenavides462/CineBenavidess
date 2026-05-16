using Microsoft.EntityFrameworkCore;
using CineBenavides.Models;

namespace CineBenavides.Data
{
    public class CineContext : DbContext
    {
        public CineContext(DbContextOptions<CineContext> options) : base(options) { }

        public DbSet<Usuario>     usuarios     { get; set; }
        public DbSet<Categoria>   categorias   { get; set; }
        public DbSet<Pelicula>    peliculas    { get; set; }
        public DbSet<Sala>        salas        { get; set; }
        public DbSet<Asiento>     asientos     { get; set; }
        public DbSet<Funcion>     funciones    { get; set; }
        public DbSet<Confiteria>  confiteria   { get; set; }
        public DbSet<Reserva>     reservas     { get; set; }
        public DbSet<ReservaItem> reservaItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Pelicula → Funciones (cascade)
            modelBuilder.Entity<Funcion>()
                .HasOne(f => f.Pelicula)
                .WithMany(p => p.Funciones)
                .HasForeignKey(f => f.PeliculaId)
                .OnDelete(DeleteBehavior.Cascade);

            // Sala → Funciones (cascade)
            modelBuilder.Entity<Funcion>()
                .HasOne(f => f.Sala)
                .WithMany(s => s.Funciones)
                .HasForeignKey(f => f.SalaId)
                .OnDelete(DeleteBehavior.Cascade);

            // Sala → Asientos (cascade)
            modelBuilder.Entity<Asiento>()
                .HasOne(a => a.Sala)
                .WithMany(s => s.Asientos)
                .HasForeignKey(a => a.SalaId)
                .OnDelete(DeleteBehavior.Cascade);

            // Funcion → Reservas (cascade)
            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Funcion)
                .WithMany(f => f.Reservas)
                .HasForeignKey(r => r.FuncionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Reserva → ReservaItems (cascade)
            modelBuilder.Entity<ReservaItem>()
                .HasOne(ri => ri.Reserva)
                .WithMany(r => r.Items)
                .HasForeignKey(ri => ri.ReservaId)
                .OnDelete(DeleteBehavior.Cascade);

            // Usuario → Reservas (NoAction para evitar múltiples cascade paths)
            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Usuario)
                .WithMany()
                .HasForeignKey(r => r.UsuarioId)
                .OnDelete(DeleteBehavior.NoAction);

            // Asiento → Reservas (NoAction para evitar múltiples cascade paths)
            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Asiento)
                .WithMany()
                .HasForeignKey(r => r.AsientoId)
                .OnDelete(DeleteBehavior.NoAction);

            // Confiteria → ReservaItems (NoAction para evitar múltiples cascade paths)
            modelBuilder.Entity<ReservaItem>()
                .HasOne(ri => ri.Confiteria)
                .WithMany()
                .HasForeignKey(ri => ri.ConfiteriaId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
