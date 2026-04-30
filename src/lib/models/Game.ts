import type { GamePhase } from "./GamePhase";
import type { Player } from "./Player";
import type { Theme } from "./Theme";

export interface Game {
    id: string;
    hostId: string;
    players: Player[];
    seed: number;
    theme: Theme;
    phase: GamePhase;
}