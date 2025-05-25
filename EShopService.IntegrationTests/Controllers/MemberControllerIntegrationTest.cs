using EShop.Domain.Models;
using EShop.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace EShopService.IntegrationTests.Controllers
{
    public class MemberControllerIntegrationTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;

        public MemberControllerIntegrationTest(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var dbContextOptions = services
                        .SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                    if (dbContextOptions != null)
                        services.Remove(dbContextOptions);

                    services.AddDbContext<ApplicationDbContext>(options => 
                        options.UseInMemoryDatabase("TestDb"));
                });
            });

            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task CreateAndGetMember_Works()
        {
            // Arrange
            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await dbContext.Database.EnsureDeletedAsync();
                await dbContext.Database.EnsureCreatedAsync();
            }

            var member = new Member 
            { 
                Name = "Test User",
                Email = "test@example.com",
                DateJoined = DateTime.UtcNow,
                Tier = MemberTier.Standard,
                PointsBalance = 0
            };

            // Act - Create
            var postResponse = await _client.PostAsJsonAsync("/api/members", member);
            postResponse.EnsureSuccessStatusCode();
            var created = await postResponse.Content.ReadFromJsonAsync<Member>();

            // Assert - Create
            Assert.NotNull(created);
            Assert.Equal(member.Name, created.Name);
            Assert.Equal(member.Email, created.Email);

            // Act - Get
            var getResponse = await _client.GetAsync($"/api/members/{created.Id}");
            getResponse.EnsureSuccessStatusCode();
            var fetched = await getResponse.Content.ReadFromJsonAsync<Member>();

            // Assert - Get
            Assert.NotNull(fetched);
            Assert.Equal(created.Id, fetched.Id);
            Assert.Equal(member.Name, fetched.Name);
            Assert.Equal(member.Email, fetched.Email);
        }
    }
} 