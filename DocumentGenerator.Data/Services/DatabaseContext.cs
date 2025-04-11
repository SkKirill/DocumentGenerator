using DocumentGenerator.Data.Models.Data;
using DocumentGenerator.Data.Models.DataBase;
using DocumentGenerator.Data.Models.DataBase.Output;
using DocumentGenerator.Data.Models.DataBase.Output.NoTable;
using DocumentGenerator.Data.Models.DataBase.Output.Table;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DocumentGenerator.Data.Services;

public sealed class DatabaseContext : DbContext
{
    public DbSet<Layout> Layouts { get; set; }
    public DbSet<BaseConfigurationModel> ConfigurationModels { get; set; }

    public override void Dispose()
    {
        SqliteConnection.ClearAllPools();
        base.Dispose();
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BaseConfigurationModel>()
            .HasDiscriminator<ConfigurationTypes>("ConfigurationType")
            .HasValue<ConfigurationModelPage>(ConfigurationTypes.Page)
            .HasValue<ConfigurationModelTable>(ConfigurationTypes.Table);

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=layouts.db");
}