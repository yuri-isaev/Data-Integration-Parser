using System.ComponentModel;
using ClientDataParser.Domains;
using ClientDataParser.Persistance;
using ClientDataParser.Validators;
using ClosedXML.Excel;

namespace ClientDataParser;

public partial class MainForm : Form
{
  private BindingList<Client> _clientsBindingList;
  private string _oldCardCode;
  private ClientRepository _clientRepository;

  public MainForm()
  {
    InitializeComponent();
    _clientRepository = new ClientRepository(new AppDbContext());
    LoadClients();
  }

  private void LoadClients()
  {
    try
    {
      var clients = _clientRepository.GetAllClientsOrderedByLastName();
      _clientsBindingList = new BindingList<Client>(clients);
      dataGridView.DataSource = _clientsBindingList;
    }
    catch (Exception ex)
    {
      MessageBox.Show($"Ошибка при загрузке клиентов: {ex.Message}");
    }
  }

  private void ImportFromExcel(string filePath)
  {
    var clients = ReadClientsFromExcel(filePath);
    SaveClientsToDatabase(clients, _clientRepository);
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

    return clients;
  }

  private bool TryCreateClientFromRow(IXLRow row, out Client client)
  {
    client = null!;
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


    #region Валидация полей

    if (!FieldsValidator.IsValid(cardCode, phoneMobile, lastName) ||
        !PhoneValidator.IsValid(ref phoneMobile, lastName))
      return false;
    if (!FieldsValidator.IsDateValid(birthdayString, out DateTime birthday, lastName)) return false;
    if (!FieldsValidator.IsPincodeValid(pincodeString, lastName, out int? localPincode)) return false;
    if (!FieldsValidator.IsBonusValid(bonusString, lastName, out int? bonus)) return false;
    if (!FieldsValidator.IsTurnoverValid(turnoverString, lastName, out int? turnover)) return false;

    #endregion

    genderId = string.IsNullOrWhiteSpace(genderId) ? "" : genderId;

    client = new Client
    {
      CardCode = cardCode,
      LastName = lastName,
      FirstName = firstName,
      SurName = surName,
      PhoneMobile = phoneMobile,
      Email = email,
      GenderId = genderId,
      Birthday = birthday,
      City = city,
      Pincode = localPincode,
      Bonus = bonus,
      Turnover = turnover
    };

    return true;
  }

  private void SaveClientsToDatabase(List<Client> clients, ClientRepository clientRepository)
  {
    using (var context = new AppDbContext())
    {
      foreach (var client in clients)
      {
        var existingClient = clientRepository.GetClientByCardCode(context, client.CardCode);

        if (existingClient != null)
        {
          bool updateCardCode = existingClient.CardCode != client.CardCode;

          if (updateCardCode)
          {
            clientRepository.UpdateCardCode(existingClient.CardCode, client.CardCode);
          }

          clientRepository.UpdateClient(context, client);
        }
        else
        {
          clientRepository.AddNewClient(context, client);
        }
      }

      clientRepository.SaveChanges(context);
    }
  }

  // listener method
  private void dataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
  {
    // Сохраняем старый CardCode при начале редактирования
    if (e.ColumnIndex == 0 && e.RowIndex >= 0) // Если редактируется CardCode
    {
      var client = _clientsBindingList[e.RowIndex];
      _oldCardCode = client.CardCode; // Сохраняем старое значение
    }
  }

  // listener method
  private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
  {
    if (e.RowIndex < 0) return; // Игнорируем заголовки

    dataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);

    var client = _clientsBindingList[e.RowIndex];

    if (e.ColumnIndex == 0) // Проверяем, что изменена ячейка CardCode
    {
      var newCardCode = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

      // Используем валидатор
      if (!FieldsValidator.IsValidCardCode(newCardCode, out var errorMessage))
      {
        MessageBox.Show(errorMessage);
        dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = client.CardCode; // Восстанавливаем старое значение
        return;
      }

      // Сохраняем старый CardCode для использования в дальнейшем
      string oldCardCode = _oldCardCode;

      using (var context = new AppDbContext())
      {
        // Сначала проверяем, существует ли клиент с новым CardCode
        var existingClientWithNewCode = _clientRepository.GetClientByCardCode(context, newCardCode);
        if (existingClientWithNewCode != null)
        {
          MessageBox.Show("Клиент с таким CardCode уже существует.");
          // Восстанавливаем старое значение
          client.CardCode = oldCardCode;
          dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = oldCardCode;
          return;
        }

        // Теперь находим клиента по старому CardCode
        var existingClient = _clientRepository.GetClientByCardCode(context, oldCardCode);
        if (existingClient != null)
        {
          try
          {
            // Создаем новый объект с изменённым CardCode
            var updatedClient = new Client
            {
              CardCode = newCardCode, // Новое значение CardCode
              LastName = existingClient.LastName,
              FirstName = existingClient.FirstName,
              SurName = existingClient.SurName,
              PhoneMobile = existingClient.PhoneMobile,
              Email = existingClient.Email,
              GenderId = existingClient.GenderId,
              Birthday = existingClient.Birthday,
              City = existingClient.City,
              Pincode = existingClient.Pincode,
              Bonus = existingClient.Bonus,
              Turnover = existingClient.Turnover
            };

            _clientRepository.RemoveClient(context, existingClient); // Удаляем старый объект
            _clientRepository.AddClient(context, updatedClient); // Добавляем новый объект
            _clientRepository.SaveChanges(context); // Сохраняем изменения

            // Обновляем объект в BindingList
            _clientsBindingList[e.RowIndex] = updatedClient; // Убедитесь, что BindingList обновлен
          }
          catch (Exception ex)
          {
            MessageBox.Show($"Ошибка при обновлении клиента: {ex.Message}\nStackTrace: {ex.StackTrace}");
          }
        }
      }
    }
    else
    {
      // Обработка изменений других полей
      using (var context = new AppDbContext())
      {
        var existingClient = _clientRepository.GetClientByCardCode(context, client.CardCode);
        if (existingClient != null)
        {
          try
          {
            ClientMapping(existingClient, client, false); // false, так как CardCode не изменяется
            _clientRepository.SaveChanges(context);
          }
          catch (Exception ex)
          {
            MessageBox.Show($"Ошибка при обновлении клиента: {ex.Message}\nStackTrace: {ex.StackTrace}");
          }
        }
      }
    }
  }

  private void ClientMapping(Client existingClient, Client newClient, bool updateCardCode)
  {
    if (updateCardCode) existingClient.CardCode = newClient.CardCode;
    existingClient.LastName = newClient.LastName;
    existingClient.FirstName = newClient.FirstName;
    existingClient.SurName = newClient.SurName;
    existingClient.PhoneMobile = newClient.PhoneMobile;
    existingClient.Email = newClient.Email;
    existingClient.GenderId = newClient.GenderId;
    existingClient.Birthday = newClient.Birthday;
    existingClient.City = newClient.City;
    existingClient.Pincode = newClient.Pincode;
    existingClient.Bonus = newClient.Bonus;
    existingClient.Turnover = newClient.Turnover;
  }
}
