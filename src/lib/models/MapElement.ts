import { Vector3 } from 'three';

export interface MapElement {
    id: string;
    type: string;
    basePosition: Vector3;
    positions: Vector3[];
    modelLocation: string;
    hasCollision: boolean;
}