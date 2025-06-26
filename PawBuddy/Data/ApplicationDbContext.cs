using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PawBuddy.Models;


namespace PawBuddy.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Animal> Animal { get; set; }
    public DbSet<Utilizador> Utilizador { get; set; }
    public DbSet<Doa> Doa { get; set; }
    public DbSet<IntencaoDeAdocao> Intencao { get; set; }
    public DbSet<Adotam> Adotam { get; set; }
    
    //adicionar metodo da seed
    /*protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    
        modelBuilder.Entity<Utilizador>(entity =>
        {
            entity.HasOne(u => u.IdentityUser)
                .WithMany()
                .HasForeignKey(u => u.IdentityUserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }*/
}