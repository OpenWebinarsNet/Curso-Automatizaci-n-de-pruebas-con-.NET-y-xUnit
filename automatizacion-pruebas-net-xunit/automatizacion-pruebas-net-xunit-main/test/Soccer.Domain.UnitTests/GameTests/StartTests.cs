using System;
using FluentAssertions;
using Soccer.Domain.Exceptions;
using Xunit;

namespace Soccer.Domain.UnitTests.GameTests;

public class StartTests
{
    [Fact]
    public void Given_A_Non_Started_Game_When_Starting_It_Should_Become_In_Progress()
    {
        // Given
        var id = Guid.Empty;
        var localTeamCode = "RMA";
        var awayTeamCode = "BAR";
        var sut = new Game(id, localTeamCode, awayTeamCode);

        var startedOn = new DateTime(2022, 3, 1, 18, 0, 0);

        // When
        sut.Start(startedOn);

        // Then
        sut.IsInProgress.Should().BeTrue();
        sut.IsEnded.Should().BeFalse();
        sut.StartedOn.Should().Be(startedOn);
    }

    [Fact]
    public void Given_A_Started_Game_When_Starting_It_Should_Throw_GameInProgressException()
    {
        // Given
        var id = Guid.Empty;
        var localTeamCode = "RMA";
        var awayTeamCode = "BAR";
        var sut = new Game(id, localTeamCode, awayTeamCode);
        var startedOn = new DateTime(2022, 3, 1, 18, 0, 0);
        sut.Start(startedOn);

        var anotherStartedOn = startedOn.AddSeconds(1);

        // When, Then
        Assert.Throws<GameInProgressException>(() =>
        {
            sut.Start(anotherStartedOn);
        });

        sut.StartedOn.Should().Be(startedOn);
        sut.IsInProgress.Should().BeTrue();
    }
}
