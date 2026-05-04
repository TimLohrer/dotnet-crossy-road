import { writable, type Writable } from 'svelte/store';
import { GameState } from '$lib/models/GameState';
import type { Game } from '$lib/models/Game';
import type { User } from '$lib/models/User';
import type { GameRenderer } from '$lib/GameRenderer';
import type { GameSocket } from '$lib/GameSocket';

export const user = writable<User | null>(null);

export const gameState: Writable<GameState> = writable(GameState.Singleplayer);

export const gameSocket = writable<GameSocket | null>(null);
export const wsGame = writable<Game | null>(null);
export const gameRenderer = writable<GameRenderer | null>(null);
