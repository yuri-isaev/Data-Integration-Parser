using CardsDataIntegration.Domains;
using CardsDataIntegration.Persistance;

namespace CardsDataIntegration.Contracts;

public interface IClientRepository
{
  Client GetClientByCardCode(AppDbContext context, string cardCode);
  void AddClient(AppDbContext context, Client client);
  void RemoveClient(AppDbContext context, Client client);
  void UpdateClient(AppDbContext context, Client client);
  bool ClientExists(string cardCode);
  void SaveChanges(AppDbContext context);
}
