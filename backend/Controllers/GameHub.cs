using System.Numerics;
using CrossyRoadApi.Database;
using CrossyRoadApi.Models.Database;
using CrossyRoadApi.Models.Game;
using CrossyRoadApi.Models.Game.Map;
using CrossyRoadApi.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace CrossyRoadApi.Controllers;

[Authorize]
public class GameHub(CrossyDbContext context, UserManager<CrossyUser> userContext) : Hub
{
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        await Clients.Caller.SendAsync(CrossyWsEvent.Ready);
    }

    public async Task CreateGame(int? seed = null)
    {
        var user = await userContext.GetUserAsync(Context.User!);
        var host = CrossyPlayer.Create(Context.ConnectionId, user!, new Vector3(0, 0, -2));
        var wsGame = CrossyGame.Create(host, CrossyTheme.Default, seed);
        ActiveGames.Games.Add(wsGame);

        await Clients.Caller.SendAsync(CrossyWsEvent.GameJoined, wsGame.ToDto());
        await Groups.AddToGroupAsync(host.ConnectionId, wsGame.Id.ToString());

        var startSections = new CrossyMapGenerator(wsGame).GenerateMapStart(host).Select(l => l.ToDto()).ToList();
        await Clients.Caller.SendAsync(CrossyWsEvent.NewSection, startSections);
    }

    public async Task JoinGame(string gameCode)
    {
        var wsGame = ActiveGames.Games.FirstOrDefault(x =>
            string.Equals(x.Id.ToString().Split("-")[0], gameCode, StringComparison.CurrentCultureIgnoreCase));
        if (wsGame == null || wsGame.GamePhase != CrossyGame.Phase.Created)
        {
            await Clients.Caller.SendAsync(CrossyWsEvent.GameJoinError, "Game does not exist");
            return;
        }

        var user = await userContext.GetUserAsync(Context.User!);
        if (wsGame.Players.Any(p => p.ConnectionId == Context.ConnectionId))
        {
            await Clients.Caller.SendAsync(CrossyWsEvent.GameJoinError, "You are already in this game");
            return;
        }

        var wsPlayer = CrossyPlayer.Create(Context.ConnectionId, user!, new Vector3(0, 0, -2));
        wsPlayer.CrossyGameId = wsGame.Id;
        wsGame.Players.Add(wsPlayer);

        await Groups.AddToGroupAsync(Context.ConnectionId, wsGame.Id.ToString());
        await Clients.Caller.SendAsync(CrossyWsEvent.GameJoined, wsGame.ToDto());
        await Clients.OthersInGroup(wsGame.Id.ToString())
            .SendAsync(CrossyWsEvent.PlayerJoined, wsGame.ToDto(), wsPlayer.UserId);

        var startSections = new CrossyMapGenerator(wsGame).GenerateMapStart(wsPlayer).Select(l => l.ToDto()).ToList();
        await Clients.Caller.SendAsync(CrossyWsEvent.NewSection, startSections);
    }

    public async Task StartGame(Guid gameId)
    {
        var wsGame = ActiveGames.Games.FirstOrDefault(g =>
            g.GamePhase == CrossyGame.Phase.Created &&
            g.Id == gameId && g.Players.Any(p => p.ConnectionId == Context.ConnectionId && p.UserId == g.HostId));
        if (wsGame != null)
        {
            wsGame.StartGame();
            await Clients.Group(wsGame.Id.ToString()).SendAsync(CrossyWsEvent.StartGame, wsGame.ToDto());
        }
    }

    public async Task UpdatePlayerModel(int skinId)
    {
        var user = await userContext.GetUserAsync(Context.User!);
        var wsGame = ActiveGames.Games.FirstOrDefault(g => g.Players.Any(p => p.ConnectionId == Context.ConnectionId));
        if (user == null || wsGame == null || wsGame.GamePhase != CrossyGame.Phase.Created || !user.OwnedSkins.Contains(skinId)) return;
        
        var wsPlayer = wsGame.Players.FirstOrDefault(p => p.ConnectionId == Context.ConnectionId)!;
        wsPlayer.User!.Skin = skinId;

        if (user.Skin != skinId)
        {
            user.Skin = skinId;
            await userContext.UpdateAsync(user);
        }
        
        await Clients.Group(wsGame.Id.ToString()).SendAsync(CrossyWsEvent.UpdatePlayerModel, wsPlayer.ToDto());
    }

    public async Task UpdatePlayerPosition(Guid gameId, Vector3 newPosition)
    {
        var wsGame = ActiveGames.Games.FirstOrDefault(g =>
            g.Id == gameId && g.Players.Any(p => p.ConnectionId == Context.ConnectionId));
        if (wsGame == null)
        {
            await Clients.Caller.SendAsync(CrossyWsEvent.PlayerLeft, "Game does not exist");
            return;
        }

        var wsPlayer = wsGame.Players.FirstOrDefault(p => p.ConnectionId == Context.ConnectionId)!;
        if (wsGame.GamePhase != CrossyGame.Phase.Active || !wsPlayer.IsAlive) return;

        var shouldGenerateNewSection = wsPlayer.UpdatePosition(newPosition);

        await Clients.Group(wsGame.Id.ToString()).SendAsync(CrossyWsEvent.UpdatePlayerPosition, wsPlayer.ToDto());

        if (shouldGenerateNewSection)
        {
            var section =
                new CrossyMapGenerator(wsGame).GenerateMapSection(wsPlayer.FurthestGeneratedZPosition, wsPlayer);
            wsPlayer.FurthestGeneratedZPosition += section.Count;
            await Clients.Caller
                .SendAsync(CrossyWsEvent.NewSection, section.Select(l => l.ToDto()).ToList());
        }
    }

    public async Task PlayerDeath(Guid gameId)
    {
        var wsGame = ActiveGames.Games.FirstOrDefault(g =>
            g.Id == gameId && g.Players.Any(p => p.ConnectionId == Context.ConnectionId));
        if (wsGame == null)
        {
            await Clients.Caller.SendAsync(CrossyWsEvent.PlayerLeft, "Game does not exist");
            return;
        }

        if (wsGame.GamePhase != CrossyGame.Phase.Active) return;

        var wsPlayer = wsGame.Players.FirstOrDefault(p => p.ConnectionId == Context.ConnectionId)!;
        if (!wsPlayer.IsAlive) return;
        wsPlayer.DiedAt = DateTime.UtcNow;

        var user = await userContext.GetUserAsync(Context.User!);
        if (user != null)
        {
            if (wsPlayer.Score > user.HighScore) user.HighScore = wsPlayer.Score;
            if (wsPlayer.Taler > 0) user.Taler += wsPlayer.Taler;
            await userContext.UpdateAsync(user);
        }

        await Clients.Group(wsGame.Id.ToString()).SendAsync(CrossyWsEvent.PlayerDeath, wsGame.ToDto());
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, wsGame.Id.ToString());

        if (wsGame.Players.Where(p => p.IsAlive).ToList().Count == 0)
        {
            wsGame.EndGame();
            await wsGame.SaveGame(context);
            ActiveGames.Games.Remove(wsGame);
        }
    }

    public async Task LeaveGame(Guid gameId)
    {
        var wsGame = ActiveGames.Games.FirstOrDefault(g =>
            g.Id == gameId && g.Players.Any(p => p.ConnectionId == Context.ConnectionId));
        if (wsGame != null)
        {
            var wsPlayer = wsGame.Players.FirstOrDefault(p => p.ConnectionId == Context.ConnectionId)!;
            wsGame.Players.Remove(wsPlayer);
            await Clients.OthersInGroup(wsGame.Id.ToString()).SendAsync(CrossyWsEvent.PlayerLeft, wsGame.ToDto());
            if (wsGame.Players.Where(p => p.IsAlive).ToList().Count == 0)
            {
                if (wsGame.GamePhase == CrossyGame.Phase.Active)
                {
                    wsGame.EndGame();
                    await wsGame.SaveGame(context);
                }

                ActiveGames.Games.Remove(wsGame);
            }
            else if (wsGame.GamePhase != CrossyGame.Phase.Active && wsGame.HostId == wsPlayer.UserId)
            {
                ActiveGames.Games.Remove(wsGame);
            }

            await Clients.Caller.SendAsync(CrossyWsEvent.LeaveGame, wsGame.Id);
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var wsGames = ActiveGames.Games.Where(g => g.Players.Any(p => p.ConnectionId == Context.ConnectionId)).ToList();
        wsGames.ForEach(g => _ = LeaveGame(g.Id));
        await base.OnDisconnectedAsync(exception);
    }
}