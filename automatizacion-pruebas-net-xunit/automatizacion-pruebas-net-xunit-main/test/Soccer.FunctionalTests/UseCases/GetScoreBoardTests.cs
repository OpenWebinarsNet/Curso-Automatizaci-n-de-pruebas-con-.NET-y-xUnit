using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Soccer.Application.Models;
using Xunit;

namespace Soccer.FunctionalTests.UseCases;

public class GetScoreBoardTests
    : FunctionalTest
{
    private string _url = string.Empty;
    private HttpResponseMessage _result = default!;
    private string _expectedScoreBoardResult = string.Empty;

    protected override async Task Given()
    {
        var createGameUrl = "games";
        var newGame =
            new NewGame
            {
                LocalTeamCode = "ALV",
                ForeignTeamCode = "ATH"
            };

        var createGameResponse = await HttpClient.PostAsJsonAsync(createGameUrl, newGame);
        if (!createGameResponse.IsSuccessStatusCode)
        {
            throw new Exception("Preconditions failed when creating game");
        }

        var gameId = createGameResponse.Headers.Location!.AbsolutePath.Split("/").Last();

        var updateGameProgressUrl = $"games/{gameId}";
        var gameProgress =
            new GameProgress
            {
                IsInProgress = true
            };

        var patchContent = JsonContent.Create(gameProgress);
        var updateGameProgressResponse = await HttpClient.PatchAsync(updateGameProgressUrl, patchContent);
        if (!updateGameProgressResponse.IsSuccessStatusCode)
        {
            throw new Exception("Preconditions failed when updating game progress");
        }

        var addGoalUrl = $"games/{gameId}/goals";
        var newGoal =
            new NewGoal
            {
                ScoredBy = "Sanadri",
                TeamCode = "ALV"
            };
        var addGoalResponse = await HttpClient.PostAsJsonAsync(addGoalUrl, newGoal);
        if (!addGoalResponse.IsSuccessStatusCode)
        {
            throw new Exception("Preconditions failed when adding a goal");
        }

        _url = $"games/{gameId}";
        _expectedScoreBoardResult = "ALV 1 - 0 ATH";
    }

    protected override async Task When()
    {
        _result = await HttpClient.GetAsync(_url);
    }

    [Fact]
    public void Then_It_Should_Return_200_Ok()
    {
        _result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Then_It_Should_Return_The_Expected_ScoreBoard_Result()
    {
        var responseJson = await _result.Content.ReadAsStringAsync();
        var scoreBoardValues = JsonSerializer.Deserialize<Dictionary<string, object>>(responseJson);
        scoreBoardValues!["result"].ToString().Should().Be(_expectedScoreBoardResult);
    }
}
