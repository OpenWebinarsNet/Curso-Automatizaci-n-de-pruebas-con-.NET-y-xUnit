using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Soccer.Application.Models;
using Xunit;

namespace Soccer.FunctionalTests.UseCases;

public class CreateGameTests
    : FunctionalTest
{
    private string _url = string.Empty;
    private NewGame _newGame = default!;
    private HttpResponseMessage _result = default!;

    protected override Task Given()
    {
        _url = "games";
        _newGame =
            new NewGame
            {
                LocalTeamCode = "ALV",
                ForeignTeamCode = "ATH"
            };

        return Task.CompletedTask;
    }

    protected override async Task When()
    {
        _result = await HttpClient.PostAsJsonAsync(_url, _newGame);
    }

    [Fact]
    public void Then_It_Should_Return_201_Created()
    {
        _result.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public void Then_It_Should_Return_A_Location_Header()
    {
        _result.Headers.Location.Should().NotBeNull();
    }
}
