using System.Net;
using System.Text;
using FluentAssertions;
using Newtonsoft.Json;
using StupidChat.Application.Chat.GetMessages.Models;
using StupidChat.Application.Common.Models;
using StupidChat.IntegrationTests.Fixtures;

namespace StupidChat.IntegrationTests;

[Collection("Integration Tests")]
public class GetMessagesQueryTests(WebApplicationFactoryFixture fixture)
{
    [Fact]
    public async Task Should_Return_Empty_Messages_For_New_Chat()
    {
        await using var host = fixture.CreateHost();
        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes("testuser1:password"));
        host.Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);

        var createResponse = await host.Client.PostAsync("api/chat", null);
        createResponse.IsSuccessStatusCode.Should().BeTrue();
        var chatId = await createResponse.Content.ReadAsStringAsync();
        var parsedChatId = chatId.Trim('"');

        var getResponse = await host.Client.GetAsync($"api/chat/{parsedChatId}?take=10&skip=0");

        getResponse.IsSuccessStatusCode.Should().BeTrue();
        var content = await getResponse.Content.ReadAsStringAsync();
        var page = JsonConvert.DeserializeObject<Page<MessageModel>>(content);
        
        page.Should().NotBeNull();
        page.Total.Should().Be(0);
        page.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Should_Return_NotFound_For_NonExistent_Chat()
    {
        await using var host = fixture.CreateHost();
        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes("testuser2:password"));
        host.Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);

        var nonExistentChatId = Guid.NewGuid();
        var response = await host.Client.GetAsync($"api/chat/{nonExistentChatId}?take=10&skip=0");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Should_Return_Unauthorized_When_No_Authentication()
    {
        await using var host = fixture.CreateHost();

        var chatId = Guid.NewGuid();
        var response = await host.Client.GetAsync($"api/chat/{chatId}?take=10&skip=0");

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

        var getResponse = await host2.Client.GetAsync($"api/chat/{parsedChatId}?take=10&skip=0");

        getResponse.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Should_Handle_Pagination_Parameters()
    {
        await using var host = fixture.CreateHost();
        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes("testuser3:password"));
        host.Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);

        var createResponse = await host.Client.PostAsync("api/chat", null);
        createResponse.IsSuccessStatusCode.Should().BeTrue();
        var chatId = await createResponse.Content.ReadAsStringAsync();
        var parsedChatId = chatId.Trim('"');

        var response1 = await host.Client.GetAsync($"api/chat/{parsedChatId}?take=5&skip=0");
        var response2 = await host.Client.GetAsync($"api/chat/{parsedChatId}?take=10&skip=5");

        response1.IsSuccessStatusCode.Should().BeTrue();
        response2.IsSuccessStatusCode.Should().BeTrue();
        
        var content1 = await response1.Content.ReadAsStringAsync();
        var content2 = await response2.Content.ReadAsStringAsync();
        
        content1.Should().NotBeNullOrEmpty();
        content2.Should().NotBeNullOrEmpty();
    }
}