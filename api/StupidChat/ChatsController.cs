using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ChatsController : ControllerBase
{
    private readonly IChatRepository _repository;

    public ChatsController(IChatRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Chat>> Get(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    [HttpPost("create")]
    public async Task<ActionResult<int>> Create([FromBody] Chat chat)
    {
        var id = await _repository.AddAsync(chat);
        
        return id;
    }
    
    [HttpPost("{id}/ask")]
    public async Task<ActionResult<Answer>> AskQuestion(int id, [FromBody] Question question)
    {
        var chat = await _repository.GetByIdAsync(id);
        
        if (chat == null)
        {
            return NotFound("Chat not found");
        }
        
        chat.Conversation[question.Value] = "I don't know";
        
        await _repository.Update(id, chat);

        return new Answer
        {
            Value = chat.Conversation[question.Value]
        };
    }
}