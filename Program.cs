namespace CardsDataIntegration;

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
    // Запускаем основную форму приложения
    Application.Run(new MainForm());
  }
}
