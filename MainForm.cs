using System.Globalization;
using System.Text.RegularExpressions;
using CardsDataIntegration.Domains;
using CardsDataIntegration.Persistance;
using ClosedXML.Excel;

namespace CardsDataIntegration;

public partial class MainForm : Form
{
  public MainForm()
  {
    InitializeComponent();
    LoadClients();

    // Set form properties
    // FormBorderStyle = FormBorderStyle.Sizable; // Allow resizing
    // WindowState     = FormWindowState.Normal; // Start as a normal window
    // StartPosition   = FormStartPosition.CenterScreen; // Center on screen
  }

  private void LoadClients()
  {
    try
    {
      using (var context = new AppDbContext())
      {
        //dataGridView.DataSource = context.Clients.ToList();
        dataGridView.DataSource = context.Clients.OrderByDescending(c => c.LastName).ToList();

      }
    }
    catch (Exception ex)
    {
      MessageBox.Show($"Ошибка при загрузке клиентов: {ex.Message}");
    }
  }

  private void btnImport_Click(object sender, EventArgs e)
  {
    using (OpenFileDialog openFileDialog = new OpenFileDialog())
    {
      openFileDialog.Filter = "Excel Files|*.xlsx";
      if (openFileDialog.ShowDialog() == DialogResult.OK)
      {
        ImportFromExcel(openFileDialog.FileName);
        LoadClients();
      }
    }
  }

  private void ImportFromExcel(string filePath)
  {
    var clients = ReadClientsFromExcel(filePath);
    SaveClientsToDatabase(clients);
  }


  private List<Client> ReadClientsFromExcel(string filePath)
  {
    var clients = new List<Client>();

    bool fileOpened = false;
    int attempts = 0;

    while (!fileOpened && attempts < 5)
    {
      try
      {
        using (var workbook = new XLWorkbook(filePath))
        {
          var worksheet = workbook.Worksheet(1);
          foreach (var row in worksheet.RowsUsed().Skip(1))
          {
            if (TryCreateClientFromRow(row, out Client client))
            {
              clients.Add(client);
            }
          }
        }

        fileOpened = true; // Файл успешно открыт
      }
      catch (IOException)
      {
        attempts++;
        Thread.Sleep(1000); // Ждем 1 секунду перед следующей попыткой
      }
    }

    if (!fileOpened)
    {
      MessageBox.Show("Не удалось открыть файл Excel после нескольких попыток.");
    }

    // Обращаем список клиентов перед возвратом
    //clients.Reverse();

    return clients;
  }


  private bool TryCreateClientFromRow(IXLRow row, out Client client)
{
    client = null;

    // Считываем данные из Excel
    var cardCode = row.Cell(1).GetFormattedString().Trim();
    var lastName = row.Cell(2).GetString().Trim();
    var firstName = row.Cell(3).GetString().Trim();
    var surName = row.Cell(4).GetString().Trim();
    var phoneMobile = row.Cell(5).GetString().Trim();
    var email = row.Cell(6).GetString().Trim();
    var genderId = row.Cell(7).GetString().Trim();
    var birthdayString = row.Cell(8).GetString().Trim();
    var city = row.Cell(9).GetString().Trim();
    var pincodeString = row.Cell(10).GetFormattedString().Trim();
    var bonusString = row.Cell(11).GetString().Trim();
    var turnoverString = row.Cell(12).GetString().Trim();

    // Проверяем обязательные поля
    if (string.IsNullOrWhiteSpace(cardCode) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(firstName))
    {
        return false; // Обязательные поля не заполнены
    }

    // Проверка телефона
    if (!Regex.IsMatch(phoneMobile, @"^\+\d{11}$"))
    {
        MessageBox.Show($"Некорректный формат телефона для клиента {lastName}: {phoneMobile}");
        return false;
    }

    // Парсинг даты
    if (!DateTime.TryParseExact(
          birthdayString,
          new[] { "d.M.yyyy", "M.d.yyyy", "yyyy-MM-dd", "dd.MM.yyyy", "d/MM/yyyy", "MM/dd/yyyy", "yyyy/MM/dd" },
          CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime birthday)
        )
    {
        MessageBox.Show($"Ошибка при парсинге даты для клиента {lastName}: {birthdayString}");
        return false;
    }

    // Установка пин-кода в null, если поле пустое
    int? pincode = null; // Пин-код будет nullable
    if (!string.IsNullOrWhiteSpace(pincodeString))
    {
        // Удаляем все нецифровые символы
        pincodeString = Regex.Replace(pincodeString, @"\D", "");
        
        // Парсинг пин-кода
        if (!int.TryParse(pincodeString, out int parsedPincode))
        {
            MessageBox.Show($"Некорректный формат пин-кода для клиента {lastName}: {pincodeString}");
            return false;
        }
        pincode = parsedPincode; // Присваиваем pincode значение
    }

    // Парсинг бонуса
    int bonus = string.IsNullOrWhiteSpace(bonusString) ? 0 : int.TryParse(bonusString, out bonus) ? bonus : 0;

    // Установка оборота в null, если поле пустое
    decimal? turnover = null; // Оборот будет nullable
    if (!string.IsNullOrWhiteSpace(turnoverString))
    {
        // Парсинг оборота
        if (!decimal.TryParse(turnoverString, out decimal parsedTurnover))
        {
            MessageBox.Show($"Некорректный формат оборота для клиента {lastName}: {turnoverString}");
            return false;
        }
        turnover = parsedTurnover; // Присваиваем turnover значение
    }

    // Установка значения пола по умолчанию
    genderId = string.IsNullOrWhiteSpace(genderId) ? "" : genderId;

    client = new Client
    {
      CardCode    = cardCode, 
      LastName    = lastName,
      FirstName   = firstName,
      SurName     = surName,
      PhoneMobile = phoneMobile,
      Email       = email,
      GenderId    = genderId,
      Birthday    = birthday,
      City        = city,
      Pincode     = pincode, // || null
      Bonus       = bonus,
      Turnover    = turnover // || null
    };

    return true;
  }


  private void SaveClientsToDatabase(List<Client> clients)
  {
    using (var context = new AppDbContext())
    {
      // Сортировка клиентов перед вставкой
      var orderedClients = clients.OrderBy(c => c.LastName).ToList();

      foreach (var client in orderedClients)
      {
        var existingClient = context.Clients.Find(client.CardCode);
            
        if (existingClient != null)
        {
          UpdateClient(existingClient, client);
        }
        else
        {
          context.Clients.Add(client);
        }
      }

      context.SaveChanges(); // Сохраняем все изменения за один раз
    }
  }
  

  private void UpdateClient(Client existingClient, Client newClient)
  {
    existingClient.LastName    = newClient.LastName;
    existingClient.FirstName   = newClient.FirstName;
    existingClient.SurName     = newClient.SurName;
    existingClient.PhoneMobile = newClient.PhoneMobile;
    existingClient.GenderId    = newClient.GenderId;
    existingClient.Birthday    = newClient.Birthday;
    existingClient.Pincode     = newClient.Pincode;
  }

  private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
  {
    using (var context = new AppDbContext())
    {
      var cardCode = dataGridView.Rows[e.RowIndex].Cells[0].Value.ToString().Trim();
      var client = context.Clients.Find(cardCode);
      if (client != null)
      {
        client.LastName    = dataGridView.Rows[e.RowIndex].Cells[1].Value.ToString().Trim();
        client.FirstName   = dataGridView.Rows[e.RowIndex].Cells[2].Value.ToString().Trim();
        client.SurName     = dataGridView.Rows[e.RowIndex].Cells[3].Value.ToString().Trim();
        client.PhoneMobile = dataGridView.Rows[e.RowIndex].Cells[4].Value.ToString().Trim();
        client.GenderId    = dataGridView.Rows[e.RowIndex].Cells[5].Value.ToString().Trim();

        var birthdayString = dataGridView.Rows[e.RowIndex].Cells[6].Value.ToString().Trim();
        client.Birthday = string.IsNullOrWhiteSpace(birthdayString) ? DateTime.MinValue : ParseDate(birthdayString);

        if (int.TryParse(dataGridView.Rows[e.RowIndex].Cells[7].Value.ToString().Trim(), out var pincode))
        {
          client.Pincode = pincode;
        }

        context.SaveChanges();
      }
    }
  }

  private DateTime ParseDate(string dateString)
  {
    DateTime date;
    string[] formats = {"d.M.yyyy", "M.d.yyyy", "yyyy-MM-dd", "dd.MM.yyyy", "d/MM/yyyy", "MM/dd/yyyy", "yyyy/MM/dd"};

    if (!DateTime.TryParseExact(dateString, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
    {
      throw new FormatException($"Неверный формат даты: {dateString}");
    }

    return date;
  }
}
