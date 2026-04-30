export class WebsocketEvent {
	static Ready = 'Ready';
	static Disconnect = 'Disconnect';
	static CreateGame = 'CreateGame';
	static JoinGame = 'JoinGame';
	static GameJoined = 'GameJoined';
	static GameLeft = 'GameLeft';
	static StartGame = 'StartGame';
	static PlayerJoined = 'PlayerJoined';
	static PlayerLeft = 'PlayerLeft';

	static NewSection = 'NewSection';
	static UpdatePlayerPosition = 'UpdatePlayerPosition';

	static GameJoinError = 'GameJoinError';
}