namespace CrossyRoadApi.Models.Game;

public static class CrossyWsEvent
{
    public static readonly string Ready = "Ready";
    public static readonly string Disconnect = "Disconnect";
    public static readonly string GameJoined = "GameJoined";
    public static readonly string StartGame = "StartGame";
    public static readonly string LeaveGame = "LeaveGame";
    public static readonly string PlayerJoined = "PlayerJoined";
    public static readonly string UpdatePlayerModel = "UpdatePlayerModel";
    public static readonly string PlayerDeath = "PlayerDeath";
    public static readonly string PlayerLeft = "PlayerLeft";
    public static readonly string UpdateUser = "UpdateUser";

    // Game Events:
    public static readonly string NewSection = "NewSection";
    public static readonly string UpdatePlayerPosition = "UpdatePlayerPosition";
    public static readonly string CollectTaler = "CollectTaler";

    // Errors:
    public static readonly string GameJoinError = "GameJoinError";

    // Events recived from client:
    // public static readonly string CreateGame = "CreateGame";
    // public static readonly string JoinGame = "JoinGame";
}