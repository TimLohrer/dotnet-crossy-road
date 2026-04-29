using System.Numerics;
using CrossyRoadApi.Models.Database;
using CrossyRoadApi.Models.Game;
using CrossyRoadApi.Models.Game.Map;
using CrossyRoadApi.Utils;
using Microsoft.AspNetCore.SignalR;

namespace CrossyRoadApi.Controllers;

public class GameHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        await Clients.Client(Context.ConnectionId).SendAsync(CrossyWebsocketEvent.Ready);
    }

    public async Task CreateGame()
    {
        // TODO: fetch from DB
        var user = new CrossyPlayer(Context.ConnectionId);
        var host = new CrossyGamePlayer(Context.ConnectionId, user, new Vector3(0, 0, -2));
        var game = new CrossyGameGame(host, CrossyTheme.Default, 0);
        ActiveGames.Games.Add(game);

        // TODO: game.ToDto()
        await Clients.Client(host.Id).SendAsync(CrossyWebsocketEvent.GameJoined, game);
        await Groups.AddToGroupAsync(host.Id, game.Id.ToString());
    }

    public async Task JoinGame(Guid gameId)
    {
        var game = ActiveGames.Games.FirstOrDefault(x => x.Id == gameId);
        if (game == null)
        {
            await Disconnect();
            return;
        }

        var user = new CrossyPlayer(Context.ConnectionId);
        var player = new CrossyGamePlayer(Context.ConnectionId, user, new Vector3(0, 0, -2));
        game.Players.Add(player);

        await Groups.AddToGroupAsync(Context.ConnectionId, game.Id.ToString());
        // TODO: game.ToDto()
        await Clients.Client(Context.ConnectionId).SendAsync(CrossyWebsocketEvent.GameJoined, game);
        await Clients.OthersInGroup(gameId.ToString()).SendAsync(CrossyWebsocketEvent.PlayerJoined, game);
    }

    private async Task Disconnect()
    {
        await Clients.Client(Context.ConnectionId).SendAsync(CrossyWebsocketEvent.Disconnect);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var activeGames = ActiveGames.Games.Where(g => g.Players.Any(p => p.Id == Context.ConnectionId)).ToList();
        foreach (var game in activeGames)
        {
            game.Players.Where(p => p.Id != Context.ConnectionId).Select(async p =>
                await Clients.Client(p.Id).SendAsync(CrossyWebsocketEvent.PlayerDisconnected));
            await Disconnect();
        }

        ActiveGames.Games.RemoveAll(activeGames.Contains);
        await base.OnDisconnectedAsync(exception);
    }
}