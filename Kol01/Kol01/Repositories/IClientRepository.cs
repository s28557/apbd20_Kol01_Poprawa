using Kol01.Models;

namespace Kol01.Repositories;

public interface IClientRepository
{
    public Task<bool> DoesCarExist(int id);
    public Task<bool> DoesClientExist(int id);
    public Task<Client> GetClient(int id);
    public Task<int> AddNewClient(NewClientDto newClientDto);
}