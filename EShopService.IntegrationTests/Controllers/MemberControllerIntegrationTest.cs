using EShopDomain.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace EShopService.IntegrationTests.Controllers
{
    public class MemberControllerIntegrationTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public MemberControllerIntegrationTest(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Post_And_Get_Member_Works()
        {
            // Arrange
            var member = new Member { Name = "Test User", Email = "test@example.com" };

            // Act
            var postResponse = await _client.PostAsJsonAsync("/api/member", member);
            postResponse.EnsureSuccessStatusCode();
            var created = await postResponse.Content.ReadFromJsonAsync<Member>();

            // Assert
            Assert.NotNull(created);
            Assert.Equal("Test User", created!.Name);

            // Act 2: Get by id
            var getResponse = await _client.GetAsync($"/api/member/{created.Id}");
            getResponse.EnsureSuccessStatusCode();
            var fetched = await getResponse.Content.ReadFromJsonAsync<Member>();

            // Assert 2
            Assert.NotNull(fetched);
            Assert.Equal(created.Id, fetched!.Id);
            Assert.Equal("Test User", fetched.Name);
        }
    }
} 