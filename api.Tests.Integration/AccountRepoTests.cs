using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using api.DTO;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace api.Tests.Integration;

public class AccountRepoTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _httpClient;

    public AccountRepoTests(WebApplicationFactory<Program> factory)
    {
        this._httpClient = factory.CreateDefaultClient();
    }

    [Fact]
    public async Task RegisterAccount_WhenDoesntExists()
    {
        UserAuthDataDTO login = new()
        {
            UserName = "qwerty" + DateTime.Today.DayOfYear + DateTime.Now.Minute,
            Password = "1234"
        };
        
        string postData = JsonSerializer.Serialize(login);

        var response = await this._httpClient.PostAsync(new Uri("https://localhost:7242/api/Account/register"),
            new StringContent(postData, Encoding.UTF8, "application/json"));
        
        response.Should().BeSuccessful();
    }

    [Fact]
    public async Task RegisterAccount_ThrowsBadRequest_WhenExists()
    {
        UserAuthDataDTO login = new()
        {
            UserName = "admin",
            Password = "admin1"
        };

        string postData = JsonSerializer.Serialize(login);
        
        var response = await this._httpClient.PostAsync(new Uri("https://localhost:7242/api/Account/register"),
            new StringContent(postData, Encoding.UTF8, "application/json"));
        
        response.Should().HaveStatusCode(HttpStatusCode.BadRequest);
    }
}