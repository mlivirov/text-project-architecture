using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class MemoryChatRepository : IChatRepository
{
    private List<Chat> _chats = new();

    public async Task<Chat> GetByIdAsync(int id)
    {
        return _chats.FirstOrDefault(c => c.Id == id);
    }

    public async Task AddAsync(Chat chat)
    {
        _chats.Add(chat);
    }

    public async Task<List<Chat>> SearchAsync(string keyword)
    {
        return _chats
            .Where(c => c.User.Contains(keyword) ||
                        c.Conversation.Any(x => x.Key.Contains(keyword) || 
                                                x.Value.Contains(keyword)))
            .ToList();
    }
}