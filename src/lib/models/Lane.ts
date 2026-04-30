import { Vector3 } from 'three';
import type { MapElement } from './MapElement';

export interface Lane {
    id: string;
    type: string;
    position: Vector3;
    modelLocation: string;
    elements: MapElement[];
}