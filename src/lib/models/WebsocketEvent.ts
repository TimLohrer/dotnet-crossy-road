export class WebsocketEvent {
	static Ready = 'Ready';
	static Disconnect = 'Disconnect';
	static CreateGame = 'CreateGame';
	static LeaveGame = 'LeaveGame';
	static JoinGame = 'JoinGame';
	static StartGame = 'StartGame';
	static GameJoined = 'GameJoined';
	static PlayerJoined = 'PlayerJoined';
	static UpdatePlayerModel = 'UpdatePlayerModel';
    static PlayerDeath = 'PlayerDeath';
	static PlayerLeft = 'PlayerLeft';
    
	static NewSection = 'NewSection';
	static UpdatePlayerPosition = 'UpdatePlayerPosition';

	static GameJoinError = 'GameJoinError';
}