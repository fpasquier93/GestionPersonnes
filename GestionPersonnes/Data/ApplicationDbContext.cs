using GestionPersonnes.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionPersonnes.Data
{
    public class ApplicationDbContext :DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Personne> Personnes { get; set; }
        public DbSet<Emploi> Emplois { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Personne>()
                .HasMany(p => p.Emplois)
                .WithOne(e => e.Personne)
                .HasForeignKey(e => e.PersonneId);
        }
    }
}
