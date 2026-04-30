import { writable, type Writable } from "svelte/store";
import { GameState } from "$lib/models/GameState";

export const isPlaying = writable(false);
export const gameState: Writable<GameState> = writable(GameState.Singleplayer);