using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class MemoryChatRepository : IChatRepository
{
    private List<Chat> _chats = new();
    private int _id = 0;

    public async Task<Chat> GetByIdAsync(int id)
    {
        return _chats.FirstOrDefault(c => c.Id == id);
    }

    public async Task<int> AddAsync(Chat chat)
    {
        chat.Id = _id++;
        
        _chats.Add(chat);
        
        return chat.Id;
    }

    public async Task Update(int id, Chat chat)
    {
        var oldChat = _chats.FirstOrDefault(c => c.Id == id);
        
        _chats.Remove(oldChat);
        
        _chats.Add(chat);
    }
}