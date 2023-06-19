using System;
using FluentAssertions;
using Soccer.Domain.Exceptions;
using Xunit;

namespace Soccer.Domain.UnitTests.GameTests;

public class EndTests
{
    [Fact]
    public void Given_An_In_Progress_Game_When_Ending_It_Should_Become_Ended()
    {
        // Given
        var id = Guid.Empty;
        var localTeamCode = "RMA";
        var awayTeamCode = "BAR";
        var sut = new Game(id, localTeamCode, awayTeamCode);

        var startedOn = new DateTime(2022, 3, 1, 18, 0, 0);
        sut.Start(startedOn);

        var endedOn = startedOn.AddHours(2);

        // When
        sut.End(endedOn);

        // Then
        sut.IsInProgress.Should().BeFalse();
        sut.IsEnded.Should().BeTrue();
        sut.StartedOn.Should().Be(startedOn);
        sut.EndedOn.Should().Be(endedOn);
    }

    [Fact]
    public void Given_An_Ended_Game_When_Ending_It_Should_Throw_GameNotInProgressException()
    {
        // Given
        var id = Guid.Empty;
        var localTeamCode = "RMA";
        var awayTeamCode = "BAR";
        var sut = new Game(id, localTeamCode, awayTeamCode);
        var startedOn = new DateTime(2022, 3, 1, 18, 0, 0);
        sut.Start(startedOn);
        var endedOn = startedOn.AddHours(2);
        sut.End(endedOn);

        var anotherEndedOn = endedOn.AddSeconds(1);

        // When, Then
        Assert.Throws<GameNotInProgressException>(() =>
        {
            sut.End(anotherEndedOn);
        });

        sut.IsInProgress.Should().BeFalse();
        sut.IsEnded.Should().BeTrue();
        sut.StartedOn.Should().Be(startedOn);
        sut.EndedOn.Should().Be(endedOn);
    }
}
