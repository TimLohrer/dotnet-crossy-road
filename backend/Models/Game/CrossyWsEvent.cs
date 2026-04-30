namespace CrossyRoadApi.Models.Game;

public static class CrossyWsEvent
{
    public static readonly string Ready = "Ready";
    public static readonly string Disconnect = "Disconnect";
    public static readonly string GameJoined = "GameJoined";
    public static readonly string GameLeft = "GameLeft";
    public static readonly string PlayerJoined = "PlayerJoined";
    public static readonly string PlayerLeft = "PlayerLeft";

    // Errors:
    public static readonly string GameJoinError = "GameJoinError";

    // Events recived from client:
    // public static readonly string CreateGame = "CreateGame";
    // public static readonly string JoinGame = "JoinGame";
}