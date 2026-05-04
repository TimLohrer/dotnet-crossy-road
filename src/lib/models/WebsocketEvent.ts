export class WebsocketEvent {
	static Ready = 'Ready';
	static Disconnect = 'Disconnect';
	static CreateGame = 'CreateGame';
	static LeaveGame = 'LeaveGame';
	static JoinGame = 'JoinGame';
	static StartGame = 'StartGame';
	static GameJoined = 'GameJoined';
	static GameLeft = 'GameLeft';
	static PlayerJoined = 'PlayerJoined';
	static PlayerLeft = 'PlayerLeft';
    
	static NewSection = 'NewSection';
	static UpdatePlayerPosition = 'UpdatePlayerPosition';

	static GameJoinError = 'GameJoinError';
}