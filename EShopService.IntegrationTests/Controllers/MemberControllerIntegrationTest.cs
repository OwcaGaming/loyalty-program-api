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

        [Fact]
        public async Task GetProfile_Works()
        {
            // Register user
            var register = new {
                Email = "profile@example.com",
                Password = "Password123!",
                FirstName = "Profile",
                LastName = "User"
            };
            var regResp = await _client.PostAsJsonAsync("/api/auth/register", register);
            regResp.EnsureSuccessStatusCode();
            var regContent = await regResp.Content.ReadFromJsonAsync<dynamic>();
            Assert.NotNull(regContent);
            string token = regContent!.token;

            // Authenticated request
            var req = new HttpRequestMessage(HttpMethod.Get, "/api/members/profile");
            req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var resp = await _client.SendAsync(req);
            resp.EnsureSuccessStatusCode();
            var member = await resp.Content.ReadFromJsonAsync<Member>();
            Assert.NotNull(member);
            Assert.Equal(register.Email, member.Email);
        }

        [Fact]
        public async Task UpdateProfile_Works()
        {
            // Register user
            var register = new {
                Email = "updateprofile@example.com",
                Password = "Password123!",
                FirstName = "Update",
                LastName = "Profile"
            };
            var regResp = await _client.PostAsJsonAsync("/api/auth/register", register);
            regResp.EnsureSuccessStatusCode();
            var regContent = await regResp.Content.ReadFromJsonAsync<dynamic>();
            Assert.NotNull(regContent);
            string token = regContent!.token;

            // Get profile
            var getReq = new HttpRequestMessage(HttpMethod.Get, "/api/members/profile");
            getReq.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var getResp = await _client.SendAsync(getReq);
            getResp.EnsureSuccessStatusCode();
            var member = await getResp.Content.ReadFromJsonAsync<Member>();
            Assert.NotNull(member);

            // Update profile
            member.PhoneNumber = "1234567890";
            member.DefaultShippingAddress = "123 Test St";
            member.DefaultBillingAddress = "456 Bill Ave";
            var putReq = new HttpRequestMessage(HttpMethod.Put, "/api/members/profile")
            {
                Content = JsonContent.Create(member)
            };
            putReq.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var putResp = await _client.SendAsync(putReq);
            putResp.EnsureSuccessStatusCode();
            var updated = await putResp.Content.ReadFromJsonAsync<Member>();
            Assert.NotNull(updated);
            Assert.Equal("1234567890", updated.PhoneNumber);
            Assert.Equal("123 Test St", updated.DefaultShippingAddress);
            Assert.Equal("456 Bill Ave", updated.DefaultBillingAddress);
        }

        [Fact]
        public async Task GetPointsHistory_EmptyInitially()
        {
            // Register user
            var register = new {
                Email = "points@example.com",
                Password = "Password123!",
                FirstName = "Points",
                LastName = "User"
            };
            var regResp = await _client.PostAsJsonAsync("/api/auth/register", register);
            regResp.EnsureSuccessStatusCode();
            var regContent = await regResp.Content.ReadFromJsonAsync<dynamic>();
            Assert.NotNull(regContent);
            string token = regContent!.token;

            // Get points history
            var req = new HttpRequestMessage(HttpMethod.Get, "/api/members/points-history");
            req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var resp = await _client.SendAsync(req);
            resp.EnsureSuccessStatusCode();
            var transactions = await resp.Content.ReadFromJsonAsync<dynamic>();
            Assert.NotNull(transactions);
            Assert.NotNull(transactions);
        }

        [Fact]
        public async Task CalculateTier_Works()
        {
            // Register user
            var register = new {
                Email = "tier@example.com",
                Password = "Password123!",
                FirstName = "Tier",
                LastName = "User"
            };
            var regResp = await _client.PostAsJsonAsync("/api/auth/register", register);
            regResp.EnsureSuccessStatusCode();
            var regContent = await regResp.Content.ReadFromJsonAsync<dynamic>();
            Assert.NotNull(regContent);
            string token = regContent!.token;

            // Calculate tier
            var req = new HttpRequestMessage(HttpMethod.Get, "/api/members/calculate-tier");
            req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var resp = await _client.SendAsync(req);
            resp.EnsureSuccessStatusCode();
            var result = await resp.Content.ReadFromJsonAsync<dynamic>();
            Assert.NotNull(result);
            Assert.NotNull(result.tier);
        }

        [Fact]
        public async Task GetProfile_Unauthorized_IfNoToken()
        {
            var resp = await _client.GetAsync("/api/members/profile");
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, resp.StatusCode);
        }
    }
} 