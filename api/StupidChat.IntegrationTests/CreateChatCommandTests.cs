using System.Net;
using System.Text;
using FluentAssertions;
using StupidChat.IntegrationTests.Fixtures;

namespace StupidChat.IntegrationTests;

[Collection("Integration Tests")]
public class CreateChatCommandTests(WebApplicationFactoryFixture fixture)
{
    [Fact]
    public async Task Should_Create_Chat_With_Valid_User()
    {
        await using var host = fixture.CreateHost();
        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes("testuser1:password"));
        host.Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);

        var response = await host.Client.PostAsync("api/chat", null);

        response.IsSuccessStatusCode.Should().BeTrue();
        var chatId = await response.Content.ReadAsStringAsync();
        chatId.Should().NotBeNullOrEmpty();
        Guid.TryParse(chatId.Trim('"'), out _).Should().BeTrue();
    }

    [Fact]
    public async Task Should_Return_Unauthorized_When_No_Authentication()
    {
        await using var host = fixture.CreateHost();

        var response = await host.Client.PostAsync("api/chat", null);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Should_Create_Multiple_Chats_For_Same_User()
    {
        await using var host = fixture.CreateHost();
        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes("testuser2:password"));
        host.Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);

        var response1 = await host.Client.PostAsync("api/chat", null);
        var response2 = await host.Client.PostAsync("api/chat", null);

        response1.IsSuccessStatusCode.Should().BeTrue();
        response2.IsSuccessStatusCode.Should().BeTrue();
        
        var chatId1 = await response1.Content.ReadAsStringAsync();
        var chatId2 = await response2.Content.ReadAsStringAsync();
        
        chatId1.Should().NotBe(chatId2);
        Guid.TryParse(chatId1.Trim('"'), out _).Should().BeTrue();
        Guid.TryParse(chatId2.Trim('"'), out _).Should().BeTrue();
    }

    [Fact]
    public async Task Should_Create_Different_Chats_For_Different_Users()
    {
        await using var host1 = fixture.CreateHost();
        var credentials1 = Convert.ToBase64String(Encoding.UTF8.GetBytes("user1:password"));
        host1.Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials1);
        
        await using var host2 = fixture.CreateHost();
        var credentials2 = Convert.ToBase64String(Encoding.UTF8.GetBytes("user2:password"));
        host2.Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials2);

        var response1 = await host1.Client.PostAsync("api/chat", null);
        var response2 = await host2.Client.PostAsync("api/chat", null);

        response1.IsSuccessStatusCode.Should().BeTrue();
        response2.IsSuccessStatusCode.Should().BeTrue();
        
        var chatId1 = await response1.Content.ReadAsStringAsync();
        var chatId2 = await response2.Content.ReadAsStringAsync();
        
        chatId1.Should().NotBe(chatId2);
        Guid.TryParse(chatId1.Trim('"'), out _).Should().BeTrue();
        Guid.TryParse(chatId2.Trim('"'), out _).Should().BeTrue();
    }
}