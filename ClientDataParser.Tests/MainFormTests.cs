using System.Collections.Generic;
using ClientDataParser.Contracts;
using ClientDataParser.Domains;
using ClientDataParser.Persistance;
using ClosedXML.Excel;
using Moq;

namespace ClientDataParser.Tests;

[TestFixture]
public class MainFormTests
{
  private MainForm _mainForm;
  private Mock<IClientRepository> _mockClientRepository;

  [SetUp]
  public void SetUp()
  {
    _mockClientRepository = new Mock<IClientRepository>();
    _mainForm = new MainForm(_mockClientRepository.Object);
  }

  [Test]
  public void LoadClients_Should_Load_Clients_Successfully()
  {
    // Arrange
    var clients = new List<Client>
    {
      new Client {CardCode = "123456", LastName = "Test1"},
      new Client {CardCode = "654321", LastName = "Test2"}
    };

    _mockClientRepository
      .Setup(repo => repo.GetAllClientsOrderedByLastName())
      .Returns(clients); // Ensure this is non-null

    // Act
    _mainForm.LoadClients();

    // Assert
    Assert.IsNotNull(_mainForm._clientsBindingList); // Ensure it's initialized
    Assert.AreEqual(2, _mainForm._clientsBindingList.Count);
    Assert.AreEqual("Test1", _mainForm._clientsBindingList[0].LastName);
    Assert.AreEqual("Test2", _mainForm._clientsBindingList[1].LastName);
  }

  [Test]
  public void LoadClients_Should_Initialize_With_Empty_List_When_No_Clients()
  {
    // Arrange
    _mockClientRepository
      .Setup(repo => repo.GetAllClientsOrderedByLastName())
      .Returns((List<Client>) null); // Simulate null return

    // Act
    _mainForm.LoadClients();

    // Assert
    Assert.IsNotNull(_mainForm._clientsBindingList); // Ensure it's initialized
    Assert.AreEqual(0, _mainForm._clientsBindingList.Count); // Ensure count is 0
  }

  [Test]
  public void TryCreateClientFromRow_Should_Return_True_When_Valid_Data()
  {
    // Arrange
    var row = new Mock<IXLRow>();
    row.Setup(r => r.Cell(1).GetFormattedString()).Returns("123456"); // Пустой CardCode
    row.Setup(r => r.Cell(2).GetString()).Returns(string.Empty); // Пустой LastName
    row.Setup(r => r.Cell(3).GetString()).Returns(string.Empty); // Пустой FirstName
    row.Setup(r => r.Cell(4).GetString()).Returns(string.Empty); // Пустой SurName
    row.Setup(r => r.Cell(5).GetString()).Returns("+72345678910"); // Пустой PhoneMobile
    row.Setup(r => r.Cell(6).GetString()).Returns(string.Empty); // Пустой Email
    row.Setup(r => r.Cell(7).GetString()).Returns(string.Empty); // Пустой GenderId
    row.Setup(r => r.Cell(8).GetString()).Returns("01.02.2000"); // Пустой BirthdayString
    row.Setup(r => r.Cell(9).GetString()).Returns(string.Empty); // Пустой City
    row.Setup(r => r.Cell(10).GetFormattedString()).Returns(string.Empty); // Пустой PincodeString
    row.Setup(r => r.Cell(11).GetString()).Returns(string.Empty); // Пустой BonusString
    row.Setup(r => r.Cell(12).GetString()).Returns(string.Empty); // Пустой TurnoverString

    // Act
    var result = _mainForm.TryCreateClientFromRow(row.Object, out Client client);

    // Assert
    Assert.IsTrue(result);
    Assert.IsNotNull(client);
    Assert.AreEqual("123456", client.CardCode);
    Assert.AreEqual(string.Empty, client.LastName);
  }

  [Test]
  public void SaveClientsToDatabase_Should_AddNewClient_WhenClientDoesNotExist()
  {
    // Arrange
    var client = new Client {CardCode = "123456", LastName = "Smith"};
    var clients = new List<Client> {client};

    _mockClientRepository
      .Setup(repo => repo.GetClientByCardCode(It.IsAny<AppDbContext>(), "123456"))
      .Returns((Client) null); // Клиент не существует

    // Act
    _mainForm.SaveClientsToDatabase(clients, _mockClientRepository.Object);

    // Assert
    _mockClientRepository.Verify(repo => repo.AddNewClient(It.IsAny<AppDbContext>(), client), Times.Once);
    _mockClientRepository.Verify(repo => repo.SaveChanges(It.IsAny<AppDbContext>()), Times.Once);
  }

  [Test]
  public void SaveClientsToDatabase_Should_UpdateClient_WhenClientExists()
  {
    // Arrange
    var existingClient = new Client {CardCode = "123456", LastName = "Doe"};
    var updatedClient = new Client {CardCode = "654321", LastName = "Smith"};
    var clients = new List<Client> {updatedClient};

    _mockClientRepository
      .Setup(repo => repo.GetClientByCardCode(It.IsAny<AppDbContext>(), "654321"))
      .Returns(existingClient); // Клиент существует

    // Act
    _mainForm.SaveClientsToDatabase(clients, _mockClientRepository.Object);

    // Assert
    _mockClientRepository.Verify(repo => repo.UpdateClient(It.IsAny<AppDbContext>(), updatedClient), Times.Once);
    _mockClientRepository.Verify(repo => repo.SaveChanges(It.IsAny<AppDbContext>()), Times.Once);
  }
}
