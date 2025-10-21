using System.Net;
using System.Text;
using FluentAssertions;
using Newtonsoft.Json;
using StupidChat.Application.Chat.SendMessage.Models;
using StupidChat.Domain.Entities;
using StupidChat.IntegrationTests.Fixtures;

namespace StupidChat.IntegrationTests;

[Collection("Integration Tests")]
public class SendMessageCommandTests(WebApplicationFactoryFixture fixture)
{
    [Fact]
    public async Task Should_Send_Message_To_Own_Chat()
    {
        await using var host = fixture.CreateHost();
        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes("testuser1:password"));
        host.Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);

        var createResponse = await host.Client.PostAsync("api/chat", null);
        createResponse.IsSuccessStatusCode.Should().BeTrue();
        var chatId = await createResponse.Content.ReadAsStringAsync();
        var parsedChatId = chatId.Trim('"');

        var messageContent = new MessageContent { Content = "Hello, how are you?" };
        var jsonContent = JsonConvert.SerializeObject(messageContent);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var sendResponse = await host.Client.PostAsync($"api/chat/{parsedChatId}", httpContent);

        sendResponse.IsSuccessStatusCode.Should().BeTrue();
        var responseContent = await sendResponse.Content.ReadAsStringAsync();
        var response = JsonConvert.DeserializeObject<ResponseModel>(responseContent);

        response.Should().NotBeNull();
        response.Id.Should().NotBe(Guid.Empty);
        response.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        response.Answer.Should().NotBeNull();
        response.Answer.Content.Should().Be("I don't know");
    }

    [Fact]
    public async Task Should_Return_NotFound_For_NonExistent_Chat()
    {
        await using var host = fixture.CreateHost();
        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes("testuser2:password"));
        host.Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);

        var nonExistentChatId = Guid.NewGuid();
        var messageContent = new MessageContent { Content = "Hello" };
        var jsonContent = JsonConvert.SerializeObject(messageContent);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await host.Client.PostAsync($"api/chat/{nonExistentChatId}", httpContent);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Should_Return_Unauthorized_When_No_Authentication()
    {
        await using var host = fixture.CreateHost();

        var chatId = Guid.NewGuid();
        var messageContent = new MessageContent { Content = "Hello" };
        var jsonContent = JsonConvert.SerializeObject(messageContent);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await host.Client.PostAsync($"api/chat/{chatId}", httpContent);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Should_Return_AccessViolation_For_Chat_Owned_By_Different_User()
    {
        await using var host1 = fixture.CreateHost();
        var credentials1 = Convert.ToBase64String(Encoding.UTF8.GetBytes("user1:password"));
        host1.Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials1);

        var createResponse = await host1.Client.PostAsync("api/chat", null);
        createResponse.IsSuccessStatusCode.Should().BeTrue();
        var chatId = await createResponse.Content.ReadAsStringAsync();
        var parsedChatId = chatId.Trim('"');

        await using var host2 = fixture.CreateHost();
        var credentials2 = Convert.ToBase64String(Encoding.UTF8.GetBytes("user2:password"));
        host2.Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials2);

        var messageContent = new MessageContent { Content = "Hello" };
        var jsonContent = JsonConvert.SerializeObject(messageContent);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var sendResponse = await host2.Client.PostAsync($"api/chat/{parsedChatId}", httpContent);

        sendResponse.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Should_Accept_Different_Message_Contents()
    {
        await using var host = fixture.CreateHost();
        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes("testuser3:password"));
        host.Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);

        var createResponse = await host.Client.PostAsync("api/chat", null);
        createResponse.IsSuccessStatusCode.Should().BeTrue();
        var chatId = await createResponse.Content.ReadAsStringAsync();
        var parsedChatId = chatId.Trim('"');

        var messages = new[]
        {
            "What is the weather today?",
            "How do I cook pasta?",
            "Tell me a joke",
            "What's 2+2?"
        };

        foreach (var messageText in messages)
        {
            var messageContent = new MessageContent { Content = messageText };
            var jsonContent = JsonConvert.SerializeObject(messageContent);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var sendResponse = await host.Client.PostAsync($"api/chat/{parsedChatId}", httpContent);

            sendResponse.IsSuccessStatusCode.Should().BeTrue();
            var responseContent = await sendResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<ResponseModel>(responseContent);

            response.Should().NotBeNull();
            response.Answer.Content.Should().Be("I don't know");
        }
    }

    [Fact]
    public async Task Should_Return_BadRequest_For_Invalid_MessageContent()
    {
        await using var host = fixture.CreateHost();
        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes("testuser4:password"));
        host.Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);

        var createResponse = await host.Client.PostAsync("api/chat", null);
        createResponse.IsSuccessStatusCode.Should().BeTrue();
        var chatId = await createResponse.Content.ReadAsStringAsync();
        var parsedChatId = chatId.Trim('"');

        var invalidJson = "{ invalid json }";
        var httpContent = new StringContent(invalidJson, Encoding.UTF8, "application/json");

        var response = await host.Client.PostAsync($"api/chat/{parsedChatId}", httpContent);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Should_Persist_Messages_And_Retrieve_Them()
    {
        await using var host = fixture.CreateHost();
        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes("testuser5:password"));
        host.Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);

        var createResponse = await host.Client.PostAsync("api/chat", null);
        createResponse.IsSuccessStatusCode.Should().BeTrue();
        var chatId = await createResponse.Content.ReadAsStringAsync();
        var parsedChatId = chatId.Trim('"');

        var messageContent = new MessageContent { Content = "Test message" };
        var jsonContent = JsonConvert.SerializeObject(messageContent);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var sendResponse = await host.Client.PostAsync($"api/chat/{parsedChatId}", httpContent);
        sendResponse.IsSuccessStatusCode.Should().BeTrue();

        var getResponse = await host.Client.GetAsync($"api/chat/{parsedChatId}?take=10&skip=0");
        getResponse.IsSuccessStatusCode.Should().BeTrue();
        
        var content = await getResponse.Content.ReadAsStringAsync();
        content.Should().Contain("Test message");
        content.Should().Contain("I don't know");
    }
}