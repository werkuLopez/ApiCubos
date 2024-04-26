using ApiCubos.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiCubos.Data
{
    public class CubosContext : DbContext
    {
        public CubosContext(DbContextOptions<CubosContext> options) : base(options) { }
        public DbSet<Cubo> Cubos { get; set; }
        public DbSet<CompraCubo> CompraCubos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
    }
}
