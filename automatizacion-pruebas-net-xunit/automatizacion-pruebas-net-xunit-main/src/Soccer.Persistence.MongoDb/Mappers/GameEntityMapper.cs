using System.Reflection;
using Soccer.Domain;
using Soccer.Persistence.MongoDb.Models;

namespace Soccer.Persistence.MongoDb.Mappers;

public class GameEntityMapper
{
    public Game ToGame(GameEntity gameEntity)
    {
        var game = new Game(gameEntity.Id, gameEntity.LocalTeamCode, gameEntity.AwayTeamCode);

        // Reflection para setear estado en objeto encapsulado
        var startedOnSetter = game.GetType().GetProperty(nameof(Game.StartedOn));
        startedOnSetter?.SetValue(game, gameEntity.StartedOn);

        var endedOnSetter = game.GetType().GetProperty(nameof(Game.EndedOn));
        endedOnSetter?.SetValue(game, gameEntity.EndedOn);

        var localGoals = gameEntity.LocalGoals.Select(ToGoal).ToList();
        var localTeamGoalsField = game.GetType().GetField("_localTeamGoals", BindingFlags.Instance | BindingFlags.NonPublic);
        localTeamGoalsField?.SetValue(game, localGoals);

        var awayGoals = gameEntity.AwayGoals.Select(ToGoal).ToList();
        var awayTeamGoalsField = game.GetType().GetField("_awayTeamGoals", BindingFlags.Instance|BindingFlags.NonPublic);
        awayTeamGoalsField?.SetValue(game, awayGoals);


        return game;
    }

    public GameEntity ToGameEntity(Game game)
    {
        var localGoalEntities = game.LocalTeamGoals.Select(ToGoalEntity);
        var awayGoalEntities = game.AwayTeamGoals.Select(ToGoalEntity);

        var gameEntity =
            new GameEntity
            {
                Id = game.Id,
                LocalTeamCode = game.LocalTeamCode,
                AwayTeamCode = game.AwayTeamCode,
                StartedOn = game.StartedOn,
                EndedOn = game.EndedOn,
                LocalGoals = localGoalEntities,
                AwayGoals = awayGoalEntities
            };

        return gameEntity;
    }

    private Goal ToGoal(GoalEntity goalEntity)
    {
        var goal = new Goal(goalEntity.ScoredOn, goalEntity.ScoredBy);
        return goal;
    }

    private GoalEntity ToGoalEntity(Goal goal)
    {
        var goalEntity =
            new GoalEntity
            {
                ScoredBy = goal.ScoredBy,
                ScoredOn = goal.ScoredOn
            };

        return goalEntity;
    }
}
