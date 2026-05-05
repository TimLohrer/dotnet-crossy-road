import * as THREE from "three";
import { GLTFLoader, type GLTF } from "three/examples/jsm/Addons.js";
import type { Lane } from "./models/Lane";
import type { Player } from "./models/Player";
import { gameSocket, menuState, user as userStore, wsGame } from "./stores/stateStore";
import { get } from "svelte/store";
import { MenuState } from "./models/MenuState";
import { Game } from "./models/Game";
import { GamePhase } from "./models/GamePhase";

export class GameRenderer {
	private static frustrumSize = 9;
	private static cameraOffsetZ = 10;
	private static GLTF_CACHE: { [key: string]: GLTF } = {};
	private static gltfLoader = new GLTFLoader();

	window: Window;
	container: HTMLDivElement;

	scene: THREE.Scene;
	camera: THREE.OrthographicCamera;
	renderer: THREE.WebGLRenderer;
	ambLight: THREE.AmbientLight;
	dirLight: THREE.DirectionalLight;
	mixers: THREE.AnimationMixer[] = [];
	timer: THREE.Timer = new THREE.Timer();

	objectList: THREE.Object3D[] = [];
	renderedObjects: THREE.Object3D[] = [];

	constructor(window: Window, container: HTMLDivElement) {
		this.window = window;
		this.container = container;

		const width = this.container.clientWidth,
			height = this.container.clientHeight;

		this.scene = new THREE.Scene();
		this.scene.background = new THREE.Color(0x00ffff);

		let aspect = width / height;

		this.camera = new THREE.OrthographicCamera(
			(-GameRenderer.frustrumSize * aspect) / 2,
			(GameRenderer.frustrumSize * aspect) / 2,
			GameRenderer.frustrumSize / 2,
			-GameRenderer.frustrumSize / 2,
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

	private render = () => this.renderer.render(this.scene, this.camera);

	private async loadModel(path: string, pos: THREE.Vector3, hasCollision: boolean, name?: string) {
		const fileName = path.split('/').pop() as string;
		const gltf =
			GameRenderer.GLTF_CACHE[fileName] ?? (await GameRenderer.gltfLoader.loadAsync(path));
		if (!GameRenderer.GLTF_CACHE[fileName]) {
			GameRenderer.GLTF_CACHE[fileName] = gltf;
			console.log(`Loaded and cached: ${fileName}`);
		}
		let model = gltf.scene.clone() as THREE.Object3D;

		if (path.includes('lane')) {
			model.traverse((child) => {
				if (child.isObject3D) {
					child.receiveShadow = true;
				}
			});
		} else {
			model.traverse((child) => {
				if (child.isObject3D) {
					child.castShadow = true;
					child.receiveShadow = true;
				}
			});
		}

		model.position.copy(pos);

		if (name) {
			model.name = name;
		}

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

		const idleAnimation = model.animations.find((a) => a.name.toLowerCase() == 'idle');
		if (idleAnimation) {
			const mixer = new THREE.AnimationMixer(model);
			mixer.clipAction(idleAnimation).play();
			this.mixers.push(mixer);
		}

		this.scene.add(model);
		this.renderedObjects.push(model);
	}

	public loadSection(lanes: Lane[]) {
		lanes.forEach((lane) => {
			this.loadModel(lane.modelLocation, lane.position, false, lane.type);
			lane.elements.forEach((element) => {
				this.loadModel(element.modelLocation, element.basePosition, element.hasCollision, element.type);
			});
		});
		this.render();
	}

	public async loadPlayer(player: Player) {
		const gltf = await GameRenderer.gltfLoader.loadAsync(`/models/skins/chicken.gltf`);
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

	private updatePlayerPosition(player: Player) {
		const playerObj = this.renderedObjects.find((obj) => obj.name === player.user.id);
		if (playerObj) {
			playerObj.position.copy(player.position.toVector3());
			this.render();
			get(gameSocket)!.sendPlayerPositionUpdate();
			this.cleanUpLanes(player, playerObj);
		}
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

	private cleanUpLanes(player: Player, playerObj: THREE.Object3D) {
		if (playerObj.position.z < player.position.z - 15) {
			this.scene.remove(playerObj);
			this.renderedObjects.filter((obj) => obj.name === player.user.id);
		}
	}

	private getElementAtPosition(position: THREE.Vector3): string | undefined {
		return this.renderedObjects.find(
			(obj) => Math.round(obj.position.x) == position.x && Math.round(obj.position.z) == position.z
		)?.name;
	}

	private cameraMovement(player: Player) {
		const targetZ = player.position.z - 16 + GameRenderer.cameraOffsetZ;

		const distanceZ = Math.abs(this.camera.position.z - targetZ);

		const baseSpeed = 0.02;
		const sensitivity = 0.1;

		const distanceFactor = distanceZ * sensitivity;
		const lerpAlpha = Math.min(baseSpeed + distanceFactor, 1.0);

		let tempCamPos = this.camera.position;

		if (tempCamPos.z < this.camera.position.z + (targetZ - this.camera.position.z) * lerpAlpha) {
			this.camera.position.z += (targetZ - this.camera.position.z) * lerpAlpha;
		}
	}

	private updateLight(player: Player) {
		this.dirLight.position.z = player.position.z - 3;
		this.dirLight.target.position.copy(new THREE.Vector3(0, 0, player.position.z));
		this.dirLight.target.updateMatrix();
	}

	private updateAnimations() {
		const delta = this.timer.getDelta();
		this.mixers.forEach((mixer) => mixer.update(delta));
	}

	private animate() {
		requestAnimationFrame(() => this.animate());
		if (get(wsGame)?.gamePhase == GamePhase.Active) {
			const player = Game.getPlayer(get(wsGame)!, get(userStore)!.id)!;
			this.cameraMovement(player);
			this.updateLight(player);
			this.updateAnimations();
		}
		this.render();
	}

	public async onKeyUp(e: KeyboardEvent) {
		const game = get(wsGame);
		const user = get(userStore);
		if (!game || !user) return;
		if (game.gamePhase == GamePhase.Created && game.hostId == user.id) {
			await get(gameSocket)?.startGame();
		};
		const player = Game.getPlayer(game, get(userStore)!.id)!;
		const moveDistance = 1;
		let newPosition = player.position.clone();
		if (get(menuState) != MenuState.Skins) {
			switch (e.key.toLowerCase()) {
				case 'w':
					newPosition.z += moveDistance;
					break;
				case 'arrowup':
					newPosition.z += moveDistance;
					break;
				case 's':
					newPosition.z -= moveDistance;
					break;
				case 'arrowdown':
					newPosition.z -= moveDistance;
					break;
				case 'a':
					newPosition.x += moveDistance;
					break;
				case 'arrowleft':
					newPosition.x += moveDistance;
					break;
				case 'd':
					newPosition.x -= moveDistance;
					break;
				case 'arrowright':
					newPosition.x -= moveDistance;
					break;
				case 'k':
					get(gameSocket)?.sendPlayerDeath();
					return;
				default:
					return;
			}

			if (game.gamePhase == GamePhase.Active && !this.isPlayerColliding(newPosition.toVector3())) {
				player.position = newPosition;
				this.updatePlayerPosition(player);
			}
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