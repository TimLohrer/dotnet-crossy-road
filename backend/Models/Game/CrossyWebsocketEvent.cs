namespace CrossyRoadApi.Models.Game;

public static class CrossyWebsocketEvent
{
    public static readonly string Ready = "Ready";
    public static readonly string Disconnect = "Disconnect";
    public static readonly string GameJoined = "GameJoined";
    public static readonly string PlayerJoined = "PlayerJoined";
    public static readonly string PlayerDisconnected = "PlayerDisconnected";
    
    // Events recived from client:
    // public static readonly string CreateGame = "CreateGame";
    // public static readonly string JoinGame = "JoinGame";
}