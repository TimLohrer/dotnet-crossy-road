using CrossyRoadApi.Models.Database;
using CrossyRoadApi.Models.Game.Map;
using CrossyRoadApi.Utils;
using Microsoft.AspNetCore.SignalR;

namespace CrossyRoadApi.Controllers;

public class GameHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public async Task CreateGame()
    {
        var host = new CrossyPlayer(Context.ConnectionId);
        var game = new CrossyGame(host, CrossyTheme.Default, 0);
        ActiveGames.Games.Add(game);
        
        await Groups.AddToGroupAsync(Context.ConnectionId, game.Id.ToString());
        await Clients.User(Context.ConnectionId).SendAsync("game-created", game.ToDto());
    }

    public async Task JoinGame(Guid gameId)
    {
        var game = ActiveGames.Games.FirstOrDefault(x => x.Id == gameId);
        if (game == null)
        {
            Disconnect();
            return;
        }
        
        var player = new CrossyPlayer(Context.ConnectionId);
        game.Players.Add(player);

        await Groups.AddToGroupAsync(Context.ConnectionId, game.Id.ToString());
        await Clients.User(Context.ConnectionId).SendAsync("game-joined", game.ToDto());
        await Clients.OthersInGroup(gameId.ToString()).SendAsync("player-joined", game.ToDto());
    }

    public async Task Disconnect()
    {
        Context.Abort();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var activeGames = ActiveGames.Games.Where(g => g.Players.Any(p => p.Username == Context.ConnectionId));
        foreach (var game in activeGames)
        {
            game.Players.Where(p => p.Username != Context.ConnectionId).Select(async p =>
                await Clients.User(p.Username).SendAsync("host-disconnected"));
            Disconnect();
            ActiveGames.Games.Remove(game);
        }
        await base.OnDisconnectedAsync(exception);
    }
}