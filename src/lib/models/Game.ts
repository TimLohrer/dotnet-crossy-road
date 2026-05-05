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
		// if (!game.startTime) return '0s';
		// const endTime = game.endTime ? Date.parse(game.endTime) : new Date().getUTCDate();
		// const duration = Math.floor((endTime - Date.parse(game.startTime)) / 1000);
		// const minutes = Math.floor(duration / 60);
		// const seconds = duration % 60;
		// if (minutes === 0) return `${seconds}s`;
		// return `${minutes}m ${seconds}s`;
		return 'FIXME!';
	}
}
