import type { Skin } from "./Skin";

export interface User {
    id: string;
    username: string;
    highScore: number;
    taler: number;
    skin: Skin;
    ownedSkins: Skin[];
}