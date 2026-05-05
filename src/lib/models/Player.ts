import type { UserMinimal } from './UserMinimal';
import { Skin } from './Skin';
import { Vec3 } from './Vec3';

export interface Player {
	connectionId: string;
	user: UserMinimal;
	position: Vec3;
	score: number;
	taler: number;
	diedAt: Date | null;
	isAlive: boolean;
}
