import type { MapElement } from './MapElement';
import type { Vec3 } from './Vec3';

export interface Lane {
    id: string;
    type: string;
    position: Vec3;
    modelLocation: string;
    elements: MapElement[];
}