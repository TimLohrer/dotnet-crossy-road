import * as signalR from '@microsoft/signalr';
import { WebsocketEvent } from './models/WebsocketEvent';
import { Game } from './models/Game';
import { get } from 'svelte/store';
import { gameRenderer, menuState, user as userStore, wsGame } from './stores/stateStore';
import { Vec3 } from './models/Vec3';
import type { Lane } from './models/Lane';
import type { Player } from './models/Player';
import { GameRenderer } from './GameRenderer';
import { MenuState } from './models/MenuState';
import { GamePhase } from './models/GamePhase';

export class GameSocket {
	private connection: signalR.HubConnection;

	onReady?: () => Promise<void>;

	constructor(url: string) {
		this.connection = new signalR.HubConnectionBuilder()
			.withUrl(url, {
				headers: {
					cookie: document.cookie
				}
			})
			.withAutomaticReconnect()
			.build();

		this.connection.on(WebsocketEvent.Ready, async () => {
			console.log('Connected to CrossyWS');
			if (this.onReady) {
				await this.onReady();
			} else {
				await this.createGame();
			}
		});

		this.connection.on(WebsocketEvent.GameJoined, async (joinedGame: Game) => {
			// Create fresh renderer to clear old game state
			const oldRenderer = this.getRenderer();
			if (oldRenderer) {
				oldRenderer.container.children.item(0)?.remove();
				gameRenderer.update(() => new GameRenderer(oldRenderer.window, oldRenderer.container));
			}

			wsGame.update((game) => {
				game = joinedGame;
				game?.players.forEach((p) => {
					p.position = Vec3.fromObject(p.position);
					this.getRenderer()!.loadPlayer(p);
				});
				return game;
			});
			if (joinedGame.players.length == 1) {
				userStore.update((u) => {
					u!.id = joinedGame.hostId;
					return u;
				});
				console.log('YOU ARE HOST!');
				console.log(`GAME CODE: ${joinedGame.id.split('-')[0].toUpperCase()}`);
			}
			
			menuState.update(() => MenuState.Play);
		});

		this.connection.on(WebsocketEvent.PlayerJoined, async (newGame: Game, newPlayerId: string) => {
			wsGame.update((game) => {
				game = newGame;
				game?.players.forEach((p) => {
					p.position = Vec3.fromObject(p.position);
					if (p.user.id === newPlayerId) {
						this.getRenderer()!.loadPlayer(p);
					}
				});
				return game;
			});
		});

		this.connection.on(WebsocketEvent.StartGame, (startedGame: Game) => {
			wsGame.update((game) => {
				game = startedGame;
				game?.players.forEach((p) => {
					p.position = Vec3.fromObject(p.position);
				});
				return game;
			});
		});

		this.connection.on(WebsocketEvent.NewSection, (lanes: Lane[]) => {
			// WTF
			lanes.forEach((lane) => {
				lane.position = Vec3.fromObject(lane.position);
				lane.elements.forEach((el) => {
					el.basePosition = Vec3.fromObject(el.basePosition);
				});
			});
			this.getRenderer()!.loadSection(lanes);
		});

		this.connection.on(WebsocketEvent.UpdatePlayerModel, (updatedPlayer: Player) => {
			wsGame.update((game) => {
				let player = game?.players.find((p) => p.user.id === updatedPlayer.user.id);
				const playerIndex = game?.players.findIndex((p) => p.user.id === updatedPlayer.user.id);
				if (player && playerIndex !== undefined) {
					game!.players[playerIndex] = updatedPlayer;
				}
				return game;
			});
			this.getRenderer()!.replacePlayerModel(updatedPlayer);
		});

		this.connection.on(WebsocketEvent.UpdatePlayerPosition, (newPlayer: Player) => {
			newPlayer.position = Vec3.fromObject(newPlayer.position);
			
			wsGame.update((game) => {
				let player = game?.players.find((p) => p.user.id === newPlayer.user.id);
				const playerIndex = game?.players.findIndex((p) => p.user.id === newPlayer.user.id);
				if (player && playerIndex !== undefined) {
					game!.players[playerIndex] = newPlayer;
				}
				return game;
			});
			
			this.getRenderer()!.syncRemotePlayerPosition(newPlayer);
		});

		this.connection.on(WebsocketEvent.PlayerDeath, (newGame: Game) => {
			const deadPlayer = this.getGame()?.players.find((p) => p.user.id == newGame.players.find((np) => np.isAlive !== p.isAlive && np.user.id == p.user.id)?.user.id);
			if (deadPlayer) {
				this.getRenderer()!.removePlayer(deadPlayer.user.id);

			}

			wsGame.update((game) => {
				game = newGame;
				game?.players.forEach((p) => {
					p.position = Vec3.fromObject(p.position);
				});
				if (deadPlayer?.user.id === this.getUser()!.id) {
					game.gamePhase = GamePhase.Ended;
				}
				return game;
			});
		});

		this.connection.on(WebsocketEvent.PlayerLeft, (newGame: Game) => {
			const leftPlayerId = this.getGame()?.players.find((p) => !newGame.players.some((np) => np.user.id === p.user.id))?.user.id;
			if (!leftPlayerId) return;

			this.getRenderer()!.removePlayer(leftPlayerId);
			wsGame.update((game) => {
				game = newGame;
				game?.players.forEach((p) => {
					p.position = Vec3.fromObject(p.position);
				});
				return game;
			});
			
			if ((leftPlayerId == newGame.hostId || newGame.players.length <= 1) && newGame?.gamePhase !== GamePhase.Active) {
				menuState.update(() => MenuState.Play);
				if (leftPlayerId == newGame.hostId) {
					this.createGame();
					// TODO: INFO POPUP -> Host left game
				}
			}
		});
	}

	public async connect() {
		try {
			await this.connection.start();
		} catch (err) {
			console.error('Error starting WebSocket connection:', err);
		}
	}

	public async destroy() {
		try {
			await this.connection.stop();
		} catch (err) {
			console.error('Error stopping WebSocket connection:', err);
		}
	}

	private getGame = () => get(wsGame);
	private getUser = () => get(userStore);
	private getPlayer = () => Game.getPlayer(this.getGame()!, this.getUser()!.id);
	private getRenderer = () => get(gameRenderer);

	public async createGame(seed: number | null = null) {
		const game = this.getGame();
		if (game) {
			await this.connection.invoke(WebsocketEvent.LeaveGame, game!.id);
		}
		await this.connection.invoke(WebsocketEvent.CreateGame, seed);
	}

	public async joinGame(gameId: string) {
		const game = this.getGame();
		if (game?.id.split('-')[0].toUpperCase() === gameId.toUpperCase()) return menuState.update(() => MenuState.Play);
		if (game) {
			await this.connection.invoke(WebsocketEvent.LeaveGame, game!.id);
		}
		
		await this.connection.invoke(WebsocketEvent.JoinGame, gameId);
	}

	public async syncPlayerModel() {
		const user = this.getUser();
		const player = this.getPlayer();
		if (!user || !player || user.skin.id == player.user.skin.id) return;
		await this.connection.invoke(WebsocketEvent.UpdatePlayerModel, user.skin.id);
	}

	public async startGame() {
		const game = this.getGame();
		if (game && game.hostId == this.getUser()!.id) {
			await this.connection.invoke(WebsocketEvent.StartGame, game.id);
		}
	}

	public async sendPlayerPositionUpdate() {
		const game = this.getGame();
		const player = this.getPlayer();
		await this.connection?.invoke(WebsocketEvent.UpdatePlayerPosition, game?.id, player?.position);
	}

	public async sendPlayerDeath() {
		const game = this.getGame();
		await this.connection?.invoke(WebsocketEvent.PlayerDeath, game?.id);
	}

	public async leaveGame() {
		const game = this.getGame();
		if (game) {
			await this.connection.invoke(WebsocketEvent.LeaveGame, game!.id);
		}
		this.createGame();
		menuState.update(() => MenuState.Play);
	}
}
