import { Vector3 } from "three";
import type { UserMinimal } from "./UserMinimal";
import { Skin } from "./Skin";

export class Player {
    connectionId: string = '';
    user: UserMinimal = { id: '', username: '', highScore: 0, skin: Skin.Default };
    position: Vector3 = new Vector3();
    furthestZPosition: number = 0;
    score: number = 0;
    taler: number = 0;

    public static fromJson(json: any): Player {
        const player = new Player();
        player.connectionId = json.connectionId;
        player.user = json.user;
        player.position = new Vector3(json.position.x, json.position.y, json.position.z);
        player.furthestZPosition = json.furthestZPosition;
        player.score = json.score;
        player.taler = json.taler;
        return player;
    }
}