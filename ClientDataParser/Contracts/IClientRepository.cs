using ClientDataParser.Domains;
using ClientDataParser.Persistance;

namespace ClientDataParser.Contracts;

public interface IClientRepository
{
  Client GetClientByCardCode(AppDbContext context, string cardCode);
  List<Client> GetAllClientsOrderedByLastName();
  void AddNewClient(AppDbContext context, Client client);
  void RemoveClient(AppDbContext context, Client client);
  void UpdateClient(AppDbContext context, Client client);
  void SaveChanges(AppDbContext context);
  void UpdateCardCode(string existingClientCardCode, string clientCardCode);
}
