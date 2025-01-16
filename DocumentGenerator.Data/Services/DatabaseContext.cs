using DocumentGenerator.Data.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DocumentGenerator.Data.Services;

public sealed class DatabaseContext : DbContext
{
    public DbSet<Layout> Layouts { get; set; }
    public override void Dispose()
    {
        SqliteConnection.ClearAllPools();
        base.Dispose();
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=layouts.db");
}