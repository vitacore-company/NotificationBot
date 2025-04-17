using Microsoft.EntityFrameworkCore;
using NotificationsBot.Models.Database;

public class AppContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Projects> Projects { get; set; }

    public DbSet<NotificationTypes> NotificationTypes { get; set; }

    public DbSet<NotificationsOnProjectChat> NotificationsOnProjectChat { get; set; }

    public DbSet<Topic> Topics { get; set; }

    public AppContext(DbContextOptions<AppContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}
