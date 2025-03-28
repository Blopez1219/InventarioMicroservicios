using Microsoft.EntityFrameworkCore;
using TransaccionesService.Models;

namespace TransaccionesService.Data
{
    public class TransaccionesDbContext : DbContext
    {
        public TransaccionesDbContext(DbContextOptions<TransaccionesDbContext> options) : base(options) { }
        public DbSet<Transaccion> Transacciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Transaccion>().ToTable("Transacciones");
        }
    }
}