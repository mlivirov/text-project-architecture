using System.Collections.Generic;
using System.Threading.Tasks;

public interface IChatRepository
{
    Task<Chat> GetByIdAsync(int id);
    Task AddAsync(Chat chat);
    Task<List<Chat>> SearchAsync(string keyword);
}
