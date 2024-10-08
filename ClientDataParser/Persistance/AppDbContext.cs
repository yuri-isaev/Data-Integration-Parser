using ClientDataParser.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ClientDataParser.Persistance;

public class AppDbContext : DbContext
{
  public DbSet<Client> Clients { get; set; }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    // Загружаем настройки из файла appsettings.json
    var configuration = new ConfigurationBuilder()
      .SetBasePath(Directory.GetCurrentDirectory())
      .AddJsonFile("appsettings.json", optional: false)
      .Build();

    // Используем строку подключения из конфигурации
    optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
  }
}
