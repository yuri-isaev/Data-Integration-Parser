using ClientDataParser.Contracts;
using ClientDataParser.Persistance;
using Microsoft.Extensions.DependencyInjection;

namespace ClientDataParser;

static class Program
{
  /// <summary>
  ///  The main entry point for the application.
  /// </summary>
  [STAThread]
  static void Main()
  {
    // Включаем визуальные стили
    Application.EnableVisualStyles();
    Application.SetCompatibleTextRenderingDefault(false);

    // Настройка DI
    var serviceProvider = new ServiceCollection()
      .AddScoped<AppDbContext>() // Register Context Db
      .AddScoped<IClientRepository, ClientRepository>() // Register repository
      .AddScoped<MainForm>() // Register MainForm with DI
      .BuildServiceProvider();

    // Получение экземпляра MainForm с инъекцией зависимостей
    var mainForm = serviceProvider.GetService<MainForm>();
    
    // Check if mainForm is null
    if (mainForm == null)
    {
      MessageBox.Show("Failed to resolve MainForm.");
      return;
    }

    Application.Run(mainForm);
  }
}
