using Microsoft.EntityFrameworkCore;
using NotificationsBot.Models;

public class AppContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public AppContext(DbContextOptions<AppContext> options):base(options)
    { 
    }
}
