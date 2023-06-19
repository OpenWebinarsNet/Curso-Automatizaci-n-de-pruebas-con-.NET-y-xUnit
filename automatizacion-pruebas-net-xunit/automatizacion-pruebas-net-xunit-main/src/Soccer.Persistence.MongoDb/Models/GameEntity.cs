namespace Soccer.Persistence.MongoDb.Models;

public class GameEntity
{
    public Guid Id { get; init; }
    public string LocalTeamCode { get; init; } = string.Empty;
    public string AwayTeamCode { get; init; } = string.Empty;
    public DateTime? StartedOn { get; init; }
    public DateTime? EndedOn { get; init; }
    public IEnumerable<GoalEntity> LocalGoals { get; init; } = Enumerable.Empty<GoalEntity>();
    public IEnumerable<GoalEntity> AwayGoals { get; init; } = Enumerable.Empty<GoalEntity>();
}

public class GoalEntity
{
    public DateTime ScoredOn { get; init; }
    public string ScoredBy { get; init; } = string.Empty;
}
