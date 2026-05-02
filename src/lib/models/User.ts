import type { Skin } from "./Skin";

export interface User {
    id: string;
    username: string;
    highscore: number;
    taler: number;
    skin: Skin;
    ownedSkins: Skin[];
}