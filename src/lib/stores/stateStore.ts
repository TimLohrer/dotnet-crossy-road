import { writable, type Writable } from 'svelte/store';
import { MenuState } from '$lib/models/MenuState';
import type { Game } from '$lib/models/Game';
import type { User } from '$lib/models/User';
import type { GameRenderer } from '$lib/GameRenderer';
import type { GameSocket } from '$lib/GameSocket';
import type { Skin } from '$lib/models/Skin';

export const isDebugMode = writable(false);

export const user = writable<User | null>(null);

export const menuState: Writable<MenuState> = writable(MenuState.Play);

export const gameSocket = writable<GameSocket | null>(null);
export const wsGame = writable<Game | null>(null);
export const gameRenderer = writable<GameRenderer | null>(null);
export const skinList = writable<Skin[]>([]);