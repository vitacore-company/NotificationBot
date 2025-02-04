using Microsoft.EntityFrameworkCore;
using NotificationsBot.Models;
using MinimalTelegramBot.StateMachine.Persistence.EntityFrameworkCore;

public class AppContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<MinimalTelegramBotState> MinimalTelegramBotStates { get; set; }
    public AppContext(DbContextOptions<AppContext> options):base(options)
    { 
        Database.EnsureCreated(); 
    }
}
