using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ChatsController : ControllerBase
{
    private readonly IChatRepository _repository;

    private static int _counter = 1;

    public ChatsController(IChatRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("{id}")]
    public async Task<Chat> Get(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    [HttpPost("create")]
    public int Create([FromBody] Chat chat)
    {
        chat.Id = _counter++;
        
        _repository.AddAsync(chat);
        
        return chat.Id;
    }

    [HttpGet("search")]
    public async Task<List<Chat>> Search(string keyword)
    {
        return await _repository.SearchAsync(keyword);
    }
    
    [HttpPost("{id}/ask")]
    public async Task<string> AskQuestion(int id, [FromBody] string question)
    {
        var chat = await _repository.GetByIdAsync(id);
        
        if (chat == null)
        {
            return "Chat not found";
        }
        
        chat.Conversation[question] = "I don't know";
        
        return chat.Conversation[question];
    }
}