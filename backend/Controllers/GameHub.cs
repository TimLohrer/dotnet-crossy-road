using System.Numerics;
using CrossyRoadApi.Database;
using CrossyRoadApi.Models.Database;
using CrossyRoadApi.Models.Game;
using CrossyRoadApi.Models.Game.Map;
using CrossyRoadApi.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CrossyRoadApi.Controllers;

[Authorize]
public class GameHub(CrossyDbContext context, UserManager<CrossyUser> userContext) : Hub
{
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        await Clients.Client(Context.ConnectionId).SendAsync(CrossyWsEvent.Ready);
    }

    public async Task CreateGame()
    {
        var user = await userContext.GetUserAsync(Context.User!);
        if (user == null)
            await Clients.Client(Context.ConnectionId)
                .SendAsync(CrossyWsEvent.Disconnect, "Invalid auth, no user found!");
        var host = CrossyPlayer.Create(Context.ConnectionId, user!, new Vector3(0, 0, -2));
        var wsGame = CrossyGame.Create(host, CrossyTheme.Default);
        ActiveGames.Games.Add(wsGame);

        await Clients.Client(host.ConnectionId).SendAsync(CrossyWsEvent.GameJoined, wsGame);
        await Groups.AddToGroupAsync(host.ConnectionId, wsGame.Id.ToString());

        var startSections = new CrossyMapGenerator(wsGame).GenerateMapStart(host).Select(l => l.ToDto()).ToList();
        await Clients.Client(host.ConnectionId).SendAsync(CrossyWsEvent.NewSection, startSections);
    }

    public async Task JoinGame(Guid gameId, Guid playerId)
    {
        var wsGame = ActiveGames.Games.FirstOrDefault(x => x.Id == gameId);
        if (wsGame == null || wsGame.GamePhase != CrossyGame.Phase.Created)
        {
            await Clients.Client(Context.ConnectionId).SendAsync(CrossyWsEvent.GameJoinError, "Game does not exist");
            return;
        }

        var user = new CrossyUser
        {
            Id = playerId,
            UserName = Context.ConnectionId
        };
        var wsPlayer = CrossyPlayer.Create(Context.ConnectionId, user, new Vector3(0, 0, -2));
        wsGame.Players.Add(wsPlayer);

        await Groups.AddToGroupAsync(Context.ConnectionId, wsGame.Id.ToString());
        await Clients.Client(Context.ConnectionId).SendAsync(CrossyWsEvent.GameJoined, wsGame);
        await Clients.OthersInGroup(gameId.ToString()).SendAsync(CrossyWsEvent.PlayerJoined, wsGame, wsPlayer.User.Id);

        var startSections = new CrossyMapGenerator(wsGame).GenerateMapStart(wsPlayer).Select(l => l.ToDto()).ToList();
        await Clients.Client(Context.ConnectionId).SendAsync(CrossyWsEvent.NewSection, startSections);
    }

    public async Task StartGame(Guid gameId)
    {
        var wsGame = ActiveGames.Games.FirstOrDefault(g =>
            g.GamePhase == CrossyGame.Phase.Created &&
            g.Id == gameId && g.Players.Any(p => p.ConnectionId == Context.ConnectionId && p.User.Id == g.HostId));
        if (wsGame != null)
        {
            wsGame.GamePhase = CrossyGame.Phase.Active;
            await Clients.Group(wsGame.Id.ToString()).SendAsync(CrossyWsEvent.StartGame, wsGame);
        }
    }

    public async Task UpdatePlayerPosition(Guid gameId, Vector3 newPosition)
    {
        var wsGame = ActiveGames.Games.FirstOrDefault(g =>
            g.Id == gameId && g.Players.Any(p => p.ConnectionId == Context.ConnectionId));
        if (wsGame == null)
        {
            await Clients.Client(Context.ConnectionId).SendAsync(CrossyWsEvent.PlayerLeft, "Game does not exist");
            return;
        }

        var wsPlayer = wsGame.Players.FirstOrDefault(p => p.ConnectionId == Context.ConnectionId)!;
        if (wsGame.GamePhase != CrossyGame.Phase.Active || !wsPlayer.IsAlive) return;

        var shouldGenerateNewSection = wsPlayer.UpdatePosition(newPosition);

        await Clients.Group(wsGame.Id.ToString()).SendAsync(CrossyWsEvent.UpdatePlayerPosition, wsPlayer);

        if (shouldGenerateNewSection)
        {
            var section =
                new CrossyMapGenerator(wsGame).GenerateMapSection(wsPlayer.FurthestGeneratedZPosition, wsPlayer);
            wsPlayer.FurthestGeneratedZPosition += section.Count;
            await Clients.Client(Context.ConnectionId)
                .SendAsync(CrossyWsEvent.NewSection, section.Select(l => l.ToDto()).ToList());
        }
    }

    public async Task PlayerDeath(Guid gameId)
    {
        var wsGame = ActiveGames.Games.FirstOrDefault(g =>
            g.Id == gameId && g.Players.Any(p => p.ConnectionId == Context.ConnectionId));
        if (wsGame == null)
        {
            await Clients.Client(Context.ConnectionId).SendAsync(CrossyWsEvent.PlayerLeft, "Game does not exist");
            return;
        }

        var wsPlayer = wsGame.Players.FirstOrDefault(p => p.ConnectionId == Context.ConnectionId)!;
        if (!wsPlayer.IsAlive) return;
        wsPlayer.DiedAt = DateTime.Now;

        var player = await userContext.Users.FirstOrDefaultAsync(p => p.Id == wsPlayer.User.Id);
        if (player != null)
        {
            if (wsPlayer.Score > player.HighScore) player.HighScore = wsPlayer.Score;
            if (wsPlayer.Taler > 0) player.Taler += wsPlayer.Taler;
        }

        await Clients.OthersInGroup(wsGame.Id.ToString()).SendAsync(CrossyWsEvent.PlayerDeath, wsGame);
    }

    public async Task LeaveGame(Guid gameId)
    {
        var wsGame = ActiveGames.Games.FirstOrDefault(g =>
            g.Id == gameId && g.Players.Any(p => p.ConnectionId == Context.ConnectionId));
        if (wsGame != null)
        {
            var wsPlayer = wsGame.Players.FirstOrDefault(p => p.ConnectionId == Context.ConnectionId)!;
            await Clients.OthersInGroup(wsGame.Id.ToString()).SendAsync(CrossyWsEvent.PlayerLeft, wsPlayer.User.Id);
            wsGame.Players.Remove(wsPlayer);
            if (wsGame.Players.Count == 0) ActiveGames.Games.Remove(wsGame);
        }

        await Clients.Client(Context.ConnectionId).SendAsync(CrossyWsEvent.GameLeft);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var wsGames = ActiveGames.Games.Where(g => g.Players.Any(p => p.ConnectionId == Context.ConnectionId)).ToList();
        wsGames.ForEach(g => _ = LeaveGame(g.Id));
        await base.OnDisconnectedAsync(exception);
    }
}