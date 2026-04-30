import { Vector3 } from "three";

export class Vec3 {
    public x: number;
    public y: number;
    public z: number;

    constructor(x: number = 0, y: number = 0, z: number = 0) {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public toVector3(): Vector3 {
        return new Vector3(this.x, this.y, this.z);
    }

    public static fromVector3(vector: Vector3): Vec3 {
        return new Vec3(vector.x, vector.y, vector.z);
    }

    public static fromObject(obj: { x: number; y: number; z: number }): Vec3 {
        return new Vec3(obj.x, obj.y, obj.z);
    }

    public copy(vec: Vec3) {
        this.x = vec.x;
        this.y = vec.y;
        this.z = vec.z;
    }

    public clone(): Vec3 {
        return new Vec3(this.x, this.y, this.z);
    }
}