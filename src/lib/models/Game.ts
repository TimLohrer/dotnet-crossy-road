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
		public startTime: Date | null,
		public endTime: Date | null
	) {}

	public static getPlayer = (game: Game, playerId: string) =>
		game.players.find((p) => p.user.id === playerId);
}
