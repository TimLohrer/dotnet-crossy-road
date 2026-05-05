import { Vector3 } from 'three';
import type { Vec3 } from './Vec3';

export interface MapElement {
    id: string;
    type: string;
    basePosition: Vec3;
    modelWidth: number;
    direction: Direction;
    modelLocation: string;
    hasCollision: boolean;
}

export enum Direction {
    Right = 0,
    Left = 1,
}