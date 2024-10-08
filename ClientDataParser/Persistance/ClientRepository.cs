using ClientDataParser.Contracts;
using ClientDataParser.Domains;

namespace ClientDataParser.Persistance;

public class ClientRepository : IClientRepository
{
  private readonly AppDbContext _context;

  public ClientRepository(AppDbContext context)
  {
    _context = context;
  }

  public void RemoveClient(AppDbContext context, Client client)
  {
    context.Clients.Remove(client);
  }

  public Client GetClientByCardCode(AppDbContext context, string cardCode)
  {
    return context.Clients.Find(cardCode);
  }

  public void AddNewClient(AppDbContext context, Client client)
  {
    context.Clients.Add(client);
    context.SaveChanges();
  }

  public void UpdateClient(AppDbContext context, Client client)
  {
    var existingClient = GetClientByCardCode(context, client.CardCode);

    if (existingClient != null)
    {
      existingClient.LastName = client.LastName;
      existingClient.FirstName = client.FirstName;
      existingClient.SurName = client.SurName;
      existingClient.PhoneMobile = client.PhoneMobile;
      existingClient.Email = client.Email;
      existingClient.GenderId = client.GenderId;
      existingClient.Birthday = client.Birthday;
      existingClient.City = client.City;
      existingClient.Pincode = client.Pincode;
      existingClient.Bonus = client.Bonus;
      existingClient.Turnover = client.Turnover;
      _context.SaveChanges();
    }
    else
    {
      throw new Exception("Клиент не найден.");
    }
  }

  public void UpdateCardCode(string oldCardCode, string newCardCode)
  {
    var existingClient = GetClientByCardCode(_context, oldCardCode);
    if (existingClient != null)
    {
      existingClient.CardCode = newCardCode;
      _context.SaveChanges();
    }
  }

  public List<Client> GetAllClientsOrderedByLastName()
  {
    return _context.Clients.OrderBy(c => c.LastName).ToList() ?? new List<Client>();
  }

  public void SaveChanges(AppDbContext context)
  {
    context.SaveChanges();
  }
}
