import type { GamePhase } from './GamePhase';
import type { Player } from './Player';
import type { Theme } from './Theme';

export class Game {
	constructor(
		public id: string,
		public hostId: string,
		public players: Player[],
		public seed: number,
		public theme: Theme,
		public gamePhase: GamePhase,
		public startTime: string | null,
		public endTime: string | null
	) {}

	public static getPlayer = (game: Game, playerId: string) =>
		game.players.find((p) => p.user.id === playerId);

	public static getDuration(game: Game): string {
		if (!game.startTime) return '0s';
		const endTime = game.endTime ? new Date(game.endTime).getTime() : new Date().getTime();
		const startTime = new Date(game.startTime).getTime();
		const duration = Math.floor((endTime - startTime) / 1000);
		const minutes = Math.floor(duration / 60);
		const seconds = duration % 60;
		return `${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;
	}
}
