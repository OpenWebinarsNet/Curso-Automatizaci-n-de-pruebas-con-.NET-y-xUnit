using System.Linq.Expressions;
using MongoDB.Driver;
using Soccer.Domain;
using Soccer.Persistence.Abstractions;
using Soccer.Persistence.MongoDb.Mappers;
using Soccer.Persistence.MongoDb.Models;

namespace Soccer.Persistence.MongoDb;
public class GameRepositoryMongoDb
    : IGameRepository
{
    private readonly IMongoCollection<GameEntity> _gameCollection;
    private readonly GameEntityMapper _gameEntityMapper;

    public GameRepositoryMongoDb(
        IMongoCollection<GameEntity> gameCollection,
        GameEntityMapper gameEntityMapper)
    {
        _gameCollection = gameCollection;
        _gameEntityMapper = gameEntityMapper;
    }

    public void Upsert(Game game)
    {
        var replaceOptions =
            new ReplaceOptions
            {
                IsUpsert = true
            };

        var gameEntity = _gameEntityMapper.ToGameEntity(game);
        Expression<Func<GameEntity, bool>> filter = x => x.Id.Equals(gameEntity.Id);
        _gameCollection.ReplaceOne(filter, gameEntity, replaceOptions);
    }

    public Game GetGame(Guid id)
    {
        Expression<Func<GameEntity, bool>> filter = x => x.Id.Equals(id);
        var queryResult = _gameCollection.Find(filter);
        var gameEntity = queryResult.SingleOrDefault();
        var game = _gameEntityMapper.ToGame(gameEntity);
        return game;
    }
}
