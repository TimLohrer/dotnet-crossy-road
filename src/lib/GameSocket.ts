import * as signalR from '@microsoft/signalr';
import { WebsocketEvent } from './models/WebsocketEvent';
import { Game } from './models/Game';
import { get } from 'svelte/store';
import { gameRenderer, user as userStore, wsGame } from './stores/stateStore';
import { Vec3 } from './models/Vec3';
import type { Lane } from './models/Lane';
import type { Player } from './models/Player';
import { GameRenderer } from './GameRenderer';

export class GameSocket {
	private connection: signalR.HubConnection;

	onReady?: () => Promise<void>;

	constructor(url: string) {
		this.connection = new signalR.HubConnectionBuilder()
			.withUrl(url)
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
				gameRenderer.set(new GameRenderer(oldRenderer.window, oldRenderer.container));
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
			}
			console.log(joinedGame.id);
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

		this.connection.on(WebsocketEvent.NewSection, (lanes: Lane[]) => this.getRenderer()!.loadSection(lanes));

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
			
			this.getRenderer()!
				.renderedObjects.find((obj) => obj.name === newPlayer.user.id)!
				.position.copy(newPlayer.position.toVector3());
		});

		this.connection.on(WebsocketEvent.PlayerLeft, (playerId: string) =>
			this.getRenderer()!.removePlayer(playerId)
		);
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

	public async createGame() {
		await this.connection.invoke(WebsocketEvent.CreateGame);
	}

	public async joinGame(gameId: string) {
		const game = this.getGame();
		if (game) {
			await this.connection.invoke(WebsocketEvent.LeaveGame, game!.id);
		}
		await this.connection.invoke(WebsocketEvent.JoinGame, gameId, this.getUser()!.id);
	}

	public sendPlayerPositionUpdate() {
		const game = this.getGame();
		const player = this.getPlayer();
		this.connection?.invoke(WebsocketEvent.UpdatePlayerPosition, game?.id, player?.position);
	}
}
