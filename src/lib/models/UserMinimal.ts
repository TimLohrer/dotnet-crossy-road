import type { Skin } from "./Skin";

export interface UserMinimal {
    id: string;
    username: string;
    highScore: number;
    skin: Skin;
}