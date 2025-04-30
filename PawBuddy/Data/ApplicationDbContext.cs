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
}