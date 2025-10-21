using System.Reflection;
using Microsoft.EntityFrameworkCore;
using StupidChat.Domain.Entities;

namespace StupidChat.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<Chat> Chats { get; set; }

    public DbSet<ChatMessage> ChatMessages { get; set; }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}