import type { UserMinimal } from './UserMinimal';
import { Skin } from './Skin';
import { Vec3 } from './Vec3';

export class Player {
	connectionId: string = '';
	user: UserMinimal = { id: '', username: '', highScore: 0, skin: Skin.Default };
	position: Vec3 = new Vec3();
	furthestZPosition: number = 0;
	score: number = 0;
	taler: number = 0;
}
