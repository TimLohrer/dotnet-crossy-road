import * as THREE from 'three';
import { GLTFLoader, type GLTF } from 'three/examples/jsm/Addons.js';
import type { Lane } from './models/Lane';
import type { Player } from './models/Player';
import { gameSocket, menuState, user as userStore, wsGame } from './stores/stateStore';
import { get } from 'svelte/store';
import { MenuState } from './models/MenuState';
import { Game } from './models/Game';
import { GamePhase } from './models/GamePhase';
import { Direction, type MapElement } from './models/MapElement';

export class GameRenderer {
	private static frustrumSize = 9;
	private static cameraOffsetZ = 10;
	private static moveAnimationDurationMs = 125;
	private static jumpHeight = 0.35;
	private static cleanupDistanceBehindPlayer = 18;
	private static GLTF_CACHE: { [key: string]: GLTF } = {};
	private static gltfLoader = new GLTFLoader();
	private static readonly IDLE_CAMERA_PUSH_DELAY_MS = 200;
	private static readonly IDLE_DEATH_TIME_MS = 8000;
	private static readonly IDLE_CAMERA_PUSH_SPEED = 0.3;
	private lastMoveTime: number = performance.now();

	window: Window;
	container: HTMLDivElement;

	scene: THREE.Scene;
	camera: THREE.OrthographicCamera;
	renderer: THREE.WebGLRenderer;
	ambLight: THREE.AmbientLight;
	dirLight: THREE.DirectionalLight;
	mixers: THREE.AnimationMixer[] = [];
	timer: THREE.Timer = new THREE.Timer();

	private static readonly LANE_MIN_X = -15;
	private static readonly LANE_MAX_X = 15;

	objectList: THREE.Object3D[] = [];
	renderedObjects: THREE.Object3D[] = [];
	elements: { [key: string]: MapElement } = {};
	private moveAnimations: {
		[playerId: string]: {
			// XZ delta to apply (relative to position at animation start)
			deltaX: number;
			deltaZ: number;
			// World X/Z of the player at the moment the animation was queued,
			// used to reconstruct absolute target each frame.
			originX: number;
			originZ: number;
			// Absolute target Z (static, never changes)
			endZ: number;
			startY: number;
			yRotationStart: number;
			yRotationEnd: number;
			startedAt: number;
			durationMs: number;
		};
	} = {};

	constructor(window: Window, container: HTMLDivElement) {
		this.window = window;
		this.container = container;

		const width = this.container.clientWidth,
			height = this.container.clientHeight;

		this.scene = new THREE.Scene();
		this.scene.background = new THREE.Color(0x00ffff);

		let aspect = width / height;

		this.camera = new THREE.OrthographicCamera(
			(-GameRenderer.frustrumSize * aspect) / 2.1,
			(GameRenderer.frustrumSize * aspect) / 2.1,
			GameRenderer.frustrumSize / 2.1,
			-GameRenderer.frustrumSize / 2.1,
			0.001,
			1000
		);
		this.camera.position.z = -6;
		this.camera.position.y = 7;
		this.camera.position.x = -1.7;
		this.camera.rotateY(Math.PI + 0.385);
		this.camera.rotateX(-Math.PI / 4.5);

		this.renderer = new THREE.WebGLRenderer({ antialias: false });
		this.renderer.setSize(window.innerWidth, window.innerHeight);
		this.renderer.shadowMap.enabled = true;
		this.renderer.shadowMap.type = THREE.PCFShadowMap;
		this.container.appendChild(this.renderer.domElement);

		this.ambLight = new THREE.AmbientLight(0xffffff, 0.3);
		this.scene.add(this.ambLight);

		this.dirLight = new THREE.DirectionalLight(0xfffffff, 2);
		this.dirLight.castShadow = true;
		this.dirLight.position.set(20, 55, -3);

		this.dirLight.shadow.normalBias = 0.05;
		this.dirLight.shadow.camera.top = 18;
		this.dirLight.shadow.camera.bottom = -18;
		this.dirLight.shadow.camera.left = -18;
		this.dirLight.shadow.camera.right = 18;
		this.dirLight.shadow.camera.near = 0.1;
		this.dirLight.shadow.camera.far = 100;
		this.dirLight.shadow.mapSize.width = 4096; // Higher resolution for better quality
		this.dirLight.shadow.mapSize.height = 4096;
		this.dirLight.shadow.intensity = 0.8;

		this.scene.add(this.dirLight);
		this.scene.add(this.dirLight.target);

		this.animate();
	}

	private getGame = () => get(wsGame);
	private getUser = () => get(userStore);
	private getSocket = () => get(gameSocket);

	private render = () => this.renderer.render(this.scene, this.camera);

	private async loadModel(
		path: string,
		pos: THREE.Vector3,
		xOffset: number,
		direction: Direction,
		hasCollision: boolean,
		name: string
	): Promise<THREE.Object3D> {
		const fileName = path.split('/').pop() as string;
		const gltf =
			GameRenderer.GLTF_CACHE[fileName] ?? (await GameRenderer.gltfLoader.loadAsync(path));
		if (!GameRenderer.GLTF_CACHE[fileName]) {
			GameRenderer.GLTF_CACHE[fileName] = gltf;
			console.log(`Loaded and cached: ${fileName}`);
		}
		let model = gltf.scene.clone() as THREE.Object3D;

		if (name.includes('lane')) {
			model.traverse((child) => {
				if (child.isObject3D) {
					child.receiveShadow = true;
				}
			});
		} else if (!name.includes('car') && !name.includes('train') && !name.includes('log')) {
			model.traverse((child) => {
				if (child.isObject3D) {
					child.castShadow = true;
					child.receiveShadow = true;
				}
			});
		}

		if (direction == Direction.Left) {
			model.rotation.y = Math.PI;
		}

		model.position.copy(pos).add(new THREE.Vector3(direction == Direction.Right ? -xOffset : xOffset, 0, 0));
		model.name = name;

		if (hasCollision) {
			// debugging: show collision boxes
			// (model as THREE.Group).children.forEach((child) => {
			// 	(child as THREE.Mesh).material = new THREE.MeshBasicMaterial({
			// 		color: 0xffffff,
			// 		wireframe: true
			// 	});
			// });
			// const cube = new THREE.Mesh(
			// 	new THREE.BoxGeometry(1, 0.05, 1),
			// 	new THREE.MeshBasicMaterial({ color: 0xff0000 })
			// );
			// cube.position.copy(model.position);
			// this.scene.add(cube);

			this.objectList.push(model);
		}

		const idleAnimation = gltf.animations.find((a) => a.name.toLowerCase() == 'idle');	
		if (idleAnimation) {
			const mixer = new THREE.AnimationMixer(model);
			mixer.clipAction(idleAnimation).play();
			this.mixers.push(mixer);
		}

		this.scene.add(model);
		this.renderedObjects.push(model);
		return model;
	}

	public loadSection(lanes: Lane[]) {
		lanes.forEach(async (lane) => {
			this.loadModel(
				lane.modelLocation,
				lane.position.toVector3(),
				0,
				Direction.Right, // Lane models are always facing right
				false,
				'lane_' + (lane.modelLocation.split('/').pop()?.split('.')[0] ?? lane.type)
			);
			if (lane.modelLocation.includes('rail')) {
				const rail_signal = await this.loadModel(
					lane.modelLocation.replace('rail', 'rail_signal'),
					lane.position.toVector3(),
					0,
					Direction.Right, // Lane models are always facing right
					false,
					'lane_rail_signal'
				);
				rail_signal.visible = false;
			}

			const elementLoads = lane.elements.map(async (element) => {
				const { uuid } = await this.loadModel(
					element.modelLocation,
					element.basePosition.toVector3(),
					element.xOffset ?? 0,
					element.direction,
					element.hasCollision,
					'element_' + (element.modelLocation.split('/').pop()?.split('.')[0] ?? element.type)
				);
				this.elements[uuid] = element;
				return uuid;
			});

			Promise.all(elementLoads).then((uuids) => {
				const min = GameRenderer.LANE_MIN_X;
				const max = GameRenderer.LANE_MAX_X;
				for (const uuid of uuids) {
					const element = this.elements[uuid];
					if (!element || element.isStatic) continue;
					const obj = this.renderedObjects.find((o) => o.uuid === uuid);
					if (!obj) continue;
					const loopLength = this.getMovingElementLoopLength(element);
					if (element.direction === Direction.Right) {
						// Drifts left — normalize into [min, min + loopLength)
						obj.position.x = min + ((obj.position.x - min) % loopLength + loopLength) % loopLength;
					} else {
						// Drifts right — normalize into (max - loopLength, max]
						obj.position.x = max - ((max - obj.position.x) % loopLength + loopLength) % loopLength;
					}
				}
			});
		});
		this.render();
	}

	public async loadPlayer(player: Player) {
		const gltf = await GameRenderer.gltfLoader.loadAsync(`/models/skins/${player.user.skin.modelName}.gltf`);
		const playerModel = gltf.scene as THREE.Object3D;
		playerModel.position.copy(player.position.toVector3());
		playerModel.name = player.user.id;
		playerModel.traverse((child) => {
			if (child.isObject3D) {
				child.castShadow = true;
				child.receiveShadow = true;
			}
		});
		this.scene.add(playerModel);
		this.render();
		this.renderedObjects.push(playerModel);
	}

	public async replacePlayerModel(player: Player) {
		const playerObj = this.renderedObjects.find((obj) => obj.name === player.user.id);
		if (!playerObj) return;
		const gltf = await GameRenderer.gltfLoader.loadAsync(`/models/skins/${player.user.skin.modelName}.gltf`);
		const newModel = gltf.scene as THREE.Object3D;
		newModel.position.copy(playerObj.position);
		newModel.rotation.copy(playerObj.rotation);
		newModel.name = player.user.id;
		newModel.traverse((child) => {
			if (child.isObject3D) {
				child.castShadow = true;
				child.receiveShadow = true;
			}
		});
		this.scene.add(newModel);
		this.scene.remove(playerObj);
		this.renderedObjects = this.renderedObjects.filter((obj) => obj.name !== player.user.id);
		this.renderedObjects.push(newModel);
	}


	private updatePlayerPosition(player: Player) {
		const playerObj = this.renderedObjects.find((obj) => obj.name === player.user.id);
		if (!playerObj) return;

		const rawTargetX = player.position.x;
		const rawTargetZ = player.position.z;
		const snappedTargetZ = Math.round(rawTargetZ);

		// Snap to the nearest slot on any log occupying the target lane,
		// or fall back to the global integer grid.
		const snappedX = this.getSnappedTargetX(rawTargetX, snappedTargetZ);

		const targetXZ = new THREE.Vector3(snappedX, 0, snappedTargetZ);
		const originX = playerObj.position.x;
		const originZ = playerObj.position.z;
		const deltaX = targetXZ.x - originX;
		const deltaZ = targetXZ.z - originZ;

		const targetYRotation = this.getTargetYRotation(
			playerObj.position,
			new THREE.Vector3(targetXZ.x, playerObj.position.y, targetXZ.z)
		);

		this.moveAnimations[player.user.id] = {
			deltaX,
			deltaZ,
			originX,
			originZ,
			endZ: targetXZ.z,
			startY: playerObj.position.y,
			yRotationStart: playerObj.rotation.y,
			yRotationEnd: targetYRotation,
			startedAt: performance.now(),
			durationMs: GameRenderer.moveAnimationDurationMs
		};
		this.render();
		this.getSocket()!.sendPlayerPositionUpdate();
		this.cleanUpInvisibleWorldObjects(player.position.z);
		if (originZ < targetXZ.z) this.lastMoveTime = performance.now();
	}

	public syncRemotePlayerPosition(player: Player) {
		const playerObj = this.renderedObjects.find((obj) => obj.name === player.user.id);
		if (!playerObj) return;
		const snappedTargetZ = Math.round(player.position.z);

		const targetXZ = new THREE.Vector3(
			this.getSnappedTargetX(player.position.x, snappedTargetZ),
			0,
			snappedTargetZ
		);
		const originX = playerObj.position.x;
		const originZ = playerObj.position.z;
		const deltaX = targetXZ.x - originX;
		const deltaZ = targetXZ.z - originZ;

		const targetYRotation = this.getTargetYRotation(
			playerObj.position,
			new THREE.Vector3(targetXZ.x, playerObj.position.y, targetXZ.z)
		);

		this.moveAnimations[player.user.id] = {
			deltaX,
			deltaZ,
			originX,
			originZ,
			endZ: targetXZ.z,
			startY: playerObj.position.y,
			yRotationStart: playerObj.rotation.y,
			yRotationEnd: targetYRotation,
			startedAt: performance.now(),
			durationMs: GameRenderer.moveAnimationDurationMs
		};
	}

	private getTargetYRotation(start: THREE.Vector3, end: THREE.Vector3): number {
		const dx = end.x - start.x;
		const dz = end.z - start.z;
		if (Math.abs(dz) >= Math.abs(dx)) {
			return dz > 0 ? 0 : Math.PI;
		}
		return dx > 0 ? Math.PI / 2 : -Math.PI / 2;
	}

	private getObjectTopY(obj: THREE.Object3D): number {
		const box = new THREE.Box3().setFromObject(obj);
		return box.max.y;
	}

	private getPlayerFeetOffset(playerObj: THREE.Object3D): number {
		const box = new THREE.Box3().setFromObject(playerObj);
		return playerObj.position.y - box.min.y;
	}

	private getLandingY(targetPosition: THREE.Vector3, playerObj: THREE.Object3D): number | null {
		if (targetPosition.x > 6 || targetPosition.x < -6) return null;

		const targetElement = this.getElementAtPosition(targetPosition);
		if (!targetElement || targetElement.name === 'lane_water') return -1;
		if (targetElement.name === 'lane_rail') return 0;

		return this.getObjectTopY(targetElement) + this.getPlayerFeetOffset(playerObj);
	}

	private isPlayerColliding(targetPosition: THREE.Vector3): boolean {
		return (
			this.objectList.find(
				(obj) => obj.position.x == targetPosition.x && obj.position.z == targetPosition.z
			) !== undefined
		);
	}

	public removePlayer(playerId: string) {
		const playerObj = this.renderedObjects.find((obj) => obj.name === playerId);
		if (playerObj) {
			this.scene.remove(playerObj);
			this.renderedObjects = this.renderedObjects.filter((obj) => obj.name !== playerId);
		}
	}

	private cleanUpInvisibleWorldObjects(playerZ: number) {
		const cutoffZ = playerZ - GameRenderer.cleanupDistanceBehindPlayer;
		const removedObjects = this.renderedObjects.filter((obj) => {
			if (obj.name === this.getUser()?.id) return false;
			const isWorldObject = obj.name.includes('lane_') || this.elements[obj.uuid] !== undefined;
			return isWorldObject && obj.position.z < cutoffZ;
		});

		if (removedObjects.length === 0) return;

		const removedUuids = new Set(removedObjects.map((obj) => obj.uuid));
		removedObjects.forEach((obj) => {
			this.scene.remove(obj);
			delete this.elements[obj.uuid];
		});

		this.renderedObjects = this.renderedObjects.filter((obj) => !removedUuids.has(obj.uuid));
		this.objectList = this.objectList.filter((obj) => !removedUuids.has(obj.uuid));
		this.mixers = this.mixers.filter((mixer) => !removedUuids.has(mixer.getRoot().uuid));
	}

	private getElementAtPosition(position: THREE.Vector3): THREE.Object3D | undefined {
		const mapElement = this.renderedObjects.find(
			(obj) => {
				const element = this.elements[obj.uuid];
				if (!element || element.hasCollision || obj.position.x > 8 || obj.position.x < -8) return false;

				// Moving elements (logs): use X/Z point-in-bounds check against the
				// element's current world bounding box so sub-tile movement doesn't
				// cause false misses regardless of the caller's context.
				if (!element.isStatic) {
					const elementBox = new THREE.Box3().setFromObject(obj);
					return (
						position.x >= elementBox.min.x &&
						position.x <= elementBox.max.x &&
						position.z >= elementBox.min.z &&
						position.z <= elementBox.max.z
					);
				}

				const playerPos = position.clone().round();
				const basePos = obj.position.clone().round();
				const occupiedPositions = [basePos];
				for (let i = 1; i < element.modelWidth; i++) {
					const offset = new THREE.Vector3();
					switch (element.direction) {
						case Direction.Right:
							offset.set(i, 0, 0);
							break;
						case Direction.Left:
							offset.set(-i, 0, 0);
							break;
					}
					occupiedPositions.push(basePos.clone().add(offset));
				}

				return occupiedPositions.some((pos) => pos.x == playerPos.x && pos.z == playerPos.z);
			}
		);

		const lane = this.renderedObjects.find(
			(obj) =>
				obj.position.x == 0 &&
				obj.position.y == 0 &&
				obj.position.z == Math.round(position.z) &&
				obj.name.includes('lane_')
		);

		return mapElement ?? lane;
	}

	private getClosestLogSlotOnLane(
		targetZ: number,
		referenceX: number,
		maxSnapDistance = 0.75
	): number | null {
		let bestSnappedX: number | null = null;
		let bestDist = Infinity;
		const laneZ = Math.round(targetZ);

		for (const obj of this.renderedObjects) {
			const element = this.elements[obj.uuid];
			if (!element || !element.modelLocation.includes('log')) continue;
			if (Math.round(element.basePosition.z) !== laneZ) continue;

			const spawnX = this.getMovingElementSpawnX(element);
			const driftX = obj.position.x - spawnX;
			const dynamicBaseX = element.basePosition.x + driftX;

			for (let i = 0; i < element.modelWidth; i++) {
				const slotX =
					element.direction === Direction.Right ? dynamicBaseX - i : dynamicBaseX + i;
				const dist = Math.abs(slotX - referenceX);
				if (dist < bestDist) {
					bestDist = dist;
					bestSnappedX = slotX;
				}
			}
		}

		if (bestSnappedX === null || bestDist > maxSnapDistance) return null;
		return bestSnappedX;
	}

	private getSnappedTargetX(rawTargetX: number, targetZ: number): number {
		const logSlotX = this.getClosestLogSlotOnLane(targetZ, rawTargetX);
		return logSlotX !== null ? logSlotX : Math.round(rawTargetX);
	}

	private isPlayerOnLog(player: Player): boolean {
		const playerObj = this.renderedObjects.find((o) => o.name === player.user.id);
		if (!playerObj) return false;
		const playerBox = new THREE.Box3().setFromObject(playerObj);
		return this.renderedObjects.some((obj) => {
			const element = this.elements[obj.uuid];
			if (!element || !element.modelLocation.includes('log')) return false;
			const elementBox = new THREE.Box3().setFromObject(obj);
			return playerBox.intersectsBox(elementBox);
		});
	}

	private getAxisOverlap(minA: number, maxA: number, minB: number, maxB: number): number {
		return Math.max(0, Math.min(maxA, maxB) - Math.max(minA, minB));
	}

	private getCollidableElementAtPosition(playerObj: THREE.Object3D): THREE.Object3D | undefined {
		const playerBox = new THREE.Box3().setFromObject(playerObj);
		const minOverlapX = (playerBox.max.x - playerBox.min.x) * 0.25;
		const minOverlapZ = (playerBox.max.z - playerBox.min.z) * 0.25;

		return this.renderedObjects.find((obj) => {
			const element = this.elements[obj.uuid];
			if (!element || !element.hasCollision || obj.position.x > 6 || obj.position.x < -6) return false;

			const elementBox = new THREE.Box3().setFromObject(obj);
			const overlapX = this.getAxisOverlap(
				playerBox.min.x,
				playerBox.max.x,
				elementBox.min.x,
				elementBox.max.x
			);
			const overlapZ = this.getAxisOverlap(
				playerBox.min.z,
				playerBox.max.z,
				elementBox.min.z,
				elementBox.max.z
			);

			return overlapX >= minOverlapX && overlapZ >= minOverlapZ;
		});
	}

	private cameraMovement(player: Player) {
		const now = performance.now()
		const idleMs = now - this.lastMoveTime;

		const baseTargetZ = player.position.z - 16 + GameRenderer.cameraOffsetZ;
		let targetZ = baseTargetZ;

		if (idleMs > GameRenderer.IDLE_CAMERA_PUSH_DELAY_MS && player.score > 0) {
			const pushElapsed = (idleMs - GameRenderer.IDLE_CAMERA_PUSH_DELAY_MS) / 1000;
			targetZ = baseTargetZ + pushElapsed * GameRenderer.IDLE_CAMERA_PUSH_SPEED;
		}

		const distanceZ = Math.abs(this.camera.position.z - targetZ);
		const baseSpeed = 0.02;
		const sensitivity = 0.1;
		const distanceFactor = distanceZ * sensitivity;
		const lerpAlpha = Math.min(baseSpeed + distanceFactor, 1.0);

		if (this.camera.position.z < targetZ) {
			this.camera.position.z += (targetZ - this.camera.position.z) * lerpAlpha;
		}
	}

	private updateLight(player: Player) {
		this.dirLight.position.z = player.position.z - 3;
		this.dirLight.target.position.copy(new THREE.Vector3(0, 0, player.position.z));
		this.dirLight.target.updateMatrix();
	}

	private handlePlayerPosition(player: Player) {
		const playerObj = this.renderedObjects.find((obj) => obj.name === player.user.id);
		if (!playerObj) return;

		// Don't evaluate death while mid-jump — the player is legitimately
		// airborne over water / between tiles during the arc.
		if (this.moveAnimations[player.user.id]) return;

		const isActiveUser = player.user.id === this.getUser()?.id;
		const currentPosition = playerObj.position.clone();
		const collidableElementAtPos = this.getCollidableElementAtPosition(playerObj);
		const elementAtPos = this.getElementAtPosition(currentPosition);
		const socket = this.getSocket()!;

		if (isActiveUser && collidableElementAtPos) {
			return socket.sendPlayerDeath();
		}

		if (isActiveUser && performance.now() - this.lastMoveTime >= GameRenderer.IDLE_DEATH_TIME_MS && player.score > 0) {
			return socket.sendPlayerDeath();
		}

		if (
			isActiveUser &&
			(!elementAtPos ||
				elementAtPos.name == 'lane_water' ||
				currentPosition.x > 6 ||
				currentPosition.x < -6)
		) {
			return socket.sendPlayerDeath();
		}
	}

	private updateAnimations() {
		this.timer.update();
		const delta = this.timer.getDelta();
		this.mixers.forEach((mixer) => mixer.update(delta));
		this.updatePlayerMoveAnimations();
		if (this.getGame()?.gamePhase == GamePhase.Active) {
			this.updateMovingElementsAnimations(delta);
		}
	}

	private getMovingElementSpawnX(element: MapElement): number {
		return element.basePosition.x +
			(element.direction === Direction.Right ? -(element.xOffset ?? 0) : (element.xOffset ?? 0));
	}

	private getMovingElementLoopLength(element: MapElement): number {
		const laneWidth = GameRenderer.LANE_MAX_X - GameRenderer.LANE_MIN_X;
		// Trains use their xOffset as the offscreen wait distance before they
		// reappear, so the loop length includes that gap. Other moving elements
		// loop seamlessly across the lane width.
		if (element.modelLocation.includes('train')) {
			return laneWidth + (element.xOffset ?? 0);
		}
		return laneWidth;
	}

	private updateMovingElementsAnimations(delta: number) {
		const min = GameRenderer.LANE_MIN_X;
		const max = GameRenderer.LANE_MAX_X;

		for (const obj of this.renderedObjects) {
			const element = this.elements[obj.uuid];
			if (!element || element.isStatic) continue;

			let dx = element.direction === Direction.Right ? -element.speed * delta : element.speed * delta;
			if (element.modelLocation.includes('log') && (obj.position.x > (6 + element.modelWidth) || obj.position.x < -6)) {
				dx *= 2; // speed up logs when they are outside the main area
			}

			const oldPos = obj.position.clone();
			obj.position.x += dx;

			// culling
			if (obj.position.x < min - 5 || obj.position.x > max + 5) {
				obj.visible = false;
			} else {
				obj.visible = true;
			}

			// Toggle rail light logic
			if (element.modelLocation.includes('train')) {
				const laneElement = this.renderedObjects.find((e) => e.position.z == obj.position.z && e.name == 'lane_rail');
				const signalLaneElement = this.renderedObjects.find((e) => e.position.z == obj.position.z && e.name == 'lane_rail_signal');
				
				const isCloseToLane = element.direction === Direction.Left ? obj.position.x > min - 65 && obj.position.x < 0 && obj.position.x - 5 < min - 65 : obj.position.x < max + 65 && obj.position.x > 0 && obj.position.x + 5 > max + 65;
				const hasLeftLane = element.direction === Direction.Left ? obj.position.x > 0 && obj.position.x - 5 < 0 : obj.position.x < 0 && obj.position.x + 5 > 0;

				if (laneElement && signalLaneElement) {
					if (isCloseToLane) {
						signalLaneElement.visible = true;
						laneElement.visible = false;
					} else if (hasLeftLane) {
						laneElement.visible = true;
						signalLaneElement.visible = false;
					}
				}
			}

			// move all alive players with log if standing on it
			if (element.modelLocation.includes('log')) {
				const elementBox = new THREE.Box3().setFromObject(obj);
				const game = this.getGame();
				if (game) {
					for (const p of game.players) {
						if (!p.isAlive) continue;
						const playerObj = this.renderedObjects.find((o) => o.name === p.user.id);
						if (!playerObj) continue;
						const playerBox = new THREE.Box3().setFromObject(playerObj);
						if (playerBox.intersectsBox(elementBox)) {
							playerObj.position.x += dx;
							p.position.x += dx;
							// Keep animation origin in sync so the drift term stays zero.
							if (this.moveAnimations[p.user.id]) {
								this.moveAnimations[p.user.id].originX += dx;
							}
						}
					}
				}
			}
			
			const loopLength = this.getMovingElementLoopLength(element);
			while (obj.position.x < min) {
				obj.position.x += loopLength;
			}

			while (obj.position.x > max) {
				obj.position.x -= loopLength;
			}
		}
	}

	private updatePlayerMoveAnimations() {
		const now = performance.now();
		Object.entries(this.moveAnimations).forEach(([playerId, animation]) => {
			const playerObj = this.renderedObjects.find((obj) => obj.name === playerId);
			if (!playerObj) {
				delete this.moveAnimations[playerId];
				return;
			}

			const elapsed = now - animation.startedAt;
			const progress = Math.min(elapsed / animation.durationMs, 1);
			const eased = 1 - Math.pow(1 - progress, 3);

			// originX is kept in sync with log carry by updateMovingElementsAnimations,
			// so we can apply the jump delta cleanly on top without any drift term.
			playerObj.position.x = animation.originX + animation.deltaX * eased;
			playerObj.position.z = animation.originZ + animation.deltaZ * eased;

			const jumpArc = Math.sin(progress * Math.PI) * GameRenderer.jumpHeight;
			const targetPos = new THREE.Vector3(
				playerObj.position.x,
				0,
				animation.endZ
			);
			const liveEndY = this.getLandingY(targetPos, playerObj);
			const baseY = liveEndY !== null ? liveEndY : animation.startY;
			playerObj.position.y = baseY + jumpArc;

			playerObj.rotation.y = this.normalizeAngle(
				animation.yRotationStart +
					this.normalizeAngle(animation.yRotationEnd - animation.yRotationStart) * eased
			);

			if (progress >= 1) {
				playerObj.position.y = liveEndY !== null ? liveEndY : animation.startY;
				delete this.moveAnimations[playerId];
			}
		});
	}

	private normalizeAngle(angle: number): number {
		const twoPi = Math.PI * 2;
		let normalized = angle % twoPi;
		if (normalized > Math.PI) normalized -= twoPi;
		if (normalized < -Math.PI) normalized += twoPi;
		return normalized;
	}

	private animate() {
		requestAnimationFrame(() => this.animate());
		this.updateAnimations();
		if (this.getGame()?.gamePhase == GamePhase.Active) {
			const player = Game.getPlayer(this.getGame()!, this.getUser()!.id)!;
			this.cameraMovement(player);
			this.updateLight(player);
			this.handlePlayerPosition(player);
			
			const isOnLog = this.isPlayerOnLog(player);
			if (player.isAlive && isOnLog) {
				// Dont kill the player due to inactivity as long as they are on a log
				this.lastMoveTime = performance.now();
			}
		}
		this.render();
	}

	public async onKeyDown(e: KeyboardEvent) {
		const game = this.getGame();
		const user = this.getUser();
		if (!game || !user) return;

		const player = Game.getPlayer(game, user.id)!;
		const pressedKey = e.key.toLowerCase();
		const isMovementKey = [
			'w',
			'a',
			's',
			'd',
			'arrowup',
			'arrowleft',
			'arrowright',
			'arrowdown'
		].includes(pressedKey);
		const moveDistance = 1;
		// Always derive target from the rendered mesh position — it's the ground
		// truth, especially when log carry has drifted the player's X.
		const playerObj = this.renderedObjects.find((obj) => obj.name === player.user.id);
		if (!playerObj) return;
		let dx = 0, dz = 0;
		if (get(menuState) !== MenuState.Play) return;
		if (
			game.gamePhase == GamePhase.Created &&
			game.hostId == user.id &&
			isMovementKey
		) {
			await this.getSocket()?.startGame();
		}

		const isXAxisKey = ['a', 'd', 'arrowleft', 'arrowright'].includes(pressedKey);
		if (
			game.gamePhase == GamePhase.Active &&
			isMovementKey &&
			this.moveAnimations[player.user.id]
		) {
			return;
		}

		if (game.gamePhase == GamePhase.Active && isXAxisKey && this.isPlayerOnLog(player)) {
			return;
		}

		switch (pressedKey) {
			case 'w':
				dz += moveDistance;
				break;
			case 'arrowup':
				dz += moveDistance;
				break;
			case 's':
				dz -= moveDistance;
				break;
			case 'arrowdown':
				dz -= moveDistance;
				break;
			case 'a':
				dx += moveDistance;
				break;
			case 'arrowleft':
				dx += moveDistance;
				break;
			case 'd':
				dx -= moveDistance;
				break;
			case 'arrowright':
				dx -= moveDistance;
				break;
			default:
				return;
		}

		// Snap the rendered position to the nearest grid tile, then add the
		// delta, snapping to a log slot only when the destination Z lane has one.
		const newPosition = player.position.clone();
		const targetZ = Math.round(playerObj.position.z) + dz;
		const rawTargetX = playerObj.position.x + dx;
		newPosition.x = this.getSnappedTargetX(rawTargetX, targetZ);
		newPosition.z = targetZ;

		if (game.gamePhase == GamePhase.Active && !this.isPlayerColliding(newPosition.toVector3())) {
			player.position = newPosition;
			this.updatePlayerPosition(player);
		}
	}

	public resize() {
		const aspect = this.window.innerWidth / this.window.innerHeight;
		this.camera.left = (-GameRenderer.frustrumSize * aspect) / 2;
		this.camera.right = (GameRenderer.frustrumSize * aspect) / 2;
		this.camera.updateProjectionMatrix();
		this.renderer.setSize(this.window.innerWidth, this.window.innerHeight);
	}
}
