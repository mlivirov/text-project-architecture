using System.Collections.Generic;
using System.Threading.Tasks;

public interface IChatRepository
{
    Task<Chat> GetByIdAsync(int id);
    Task<int> AddAsync(Chat chat);
    Task Update(int id, Chat chat);
}
