<script lang="ts">
	import type { Lane } from '$lib/models/Lane';
	import { onMount } from 'svelte';
	import * as THREE from 'three';
	import { GLTFLoader } from 'three/addons/loaders/GLTFLoader.js';
	import type { GLTF, Position } from 'three/examples/jsm/Addons.js';

	const PIXEL = 0.05;
	const gameLoop: boolean = true;
	const objectList: THREE.Object3D[] = [];
	const renderedObjects: THREE.Object3D[] = [];

	const GLTF_CACHE: { [key: string]: GLTF } = {};

	onMount(async () => {
		const width = window.innerWidth,
			height = window.innerHeight;

		const scene = new THREE.Scene();

		const frustrumSize = 9;
		let aspect = width / height;

		const camera = new THREE.OrthographicCamera(
			(-frustrumSize * aspect) / 2,
			(frustrumSize * aspect) / 2,
			frustrumSize / 2,
			-frustrumSize / 2,
			-100,
			1000
		);
		camera.position.z = -2;
		camera.position.y = 0;
		camera.position.x = 0;
		camera.rotateY(Math.PI + 0.269);
		camera.rotateX(-Math.PI / 4);

		const renderer = new THREE.WebGLRenderer({ antialias: false });
		renderer.setSize(window.innerWidth, window.innerHeight);
		document.body.appendChild(renderer.domElement);

		const light = new THREE.HemisphereLight(0xffffff, 0x444444);
		light.position.set(-20, 20, 0);
		scene.add(light);

		const loader = new GLTFLoader();

		async function loadModel(path: string, pos: THREE.Vector3, hasCollision: boolean) {
			const fileName = path.split('/').pop() as string;
			const gltf = GLTF_CACHE[fileName] ?? await loader.loadAsync(path);
			if (!GLTF_CACHE[fileName]) {
				GLTF_CACHE[fileName] = gltf;
				console.log(`Loaded and cached: ${fileName}`);
			}
			let model = gltf.scene.clone() as THREE.Object3D;
			
			model.position.x = pos.x;
			model.position.y = pos.y;
			model.position.z = pos.z;
			
			if (hasCollision) {
				// debugging: show collision boxes
				// (model as THREE.Group).children.forEach((child) => {
				// 	(child as THREE.Mesh).material = new THREE.MeshBasicMaterial({ color: 0xffffff, wireframe: true });
				// });
				// const cube = new THREE.Mesh(
				// 	new THREE.BoxGeometry(1, .05, 1),
				// 	new THREE.MeshBasicMaterial({ color: 0xff0000 })
				// );
				// cube.position.copy(model.position);
				// scene.add(cube);

				objectList.push(model);
			}

			scene.add(model);
			renderedObjects.push(model);
		}

		renderer.render(scene, camera);

		const seed = Math.floor(Math.random() * 1000000);

		async function loadLane(z: number) {
			const res = await fetch(`http://localhost:5016/api/v1/game/${z}?seed=${seed}`);
			const lane = await res.json() as Lane;
			
			loadModel(lane.modelLocation, lane.position, false);
			lane.elements.forEach((element) => {
				loadModel(element.modelLocation, element.basePosition, true);
			});
			renderer.render(scene, camera);
		}

		for (let i = -10; i < 15; i++) {
			if (i < 0) {
				loadModel('/models/default/lanes/plains.gltf', new THREE.Vector3(0, 0, i), false);
			} else {
				loadLane(i);
			}
		}

		const gltf = await loader.loadAsync(`/models/skins/chicken.gltf`);
		const player = gltf.scene as THREE.Object3D;
		player.position.z = -2;
		scene.add(player);
		renderer.render(scene, camera);

		document.addEventListener('keydown', (event) => {
			const moveDistance = 1;
			let newPosition = player.position.clone();
			switch (event.key.toLowerCase()) {
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
			}

			if (!isPlayerColliding(newPosition)) {
				if (player.position.z < newPosition.z) {
					camera.position.z += 1;
					loadLane(newPosition.z + 15);
				}
				player.position.copy(newPosition);
				renderer.render(scene, camera);
				cleanUpLanes();
			}
		});

		function isPlayerColliding(targetPosition: THREE.Vector3): boolean {
			for (const object of objectList) {
				if (object.position.x == targetPosition.x && object.position.z == targetPosition.z) {
					return true;
				}
			}
			return false;
		}

		function cleanUpLanes() {
			for (let i = renderedObjects.length - 1; i >= 0; i--) {
				const obj = renderedObjects[i];
				if (obj.position.z < player.position.z - 10) {
					scene.remove(obj);
					renderedObjects.splice(i, 1);
					console.log(`Removed object at z=${obj.position.z}`);
				}
			}
		}

		function animate() {
			requestAnimationFrame(animate);
			renderer.render(scene, camera);
		}
		animate();

		window.addEventListener('resize', () => {
			aspect = window.innerWidth / window.innerHeight;
			camera.left = (-frustrumSize * aspect) / 2;
			camera.right = (frustrumSize * aspect) / 2;
			camera.updateProjectionMatrix();
			renderer.setSize(window.innerWidth, window.innerHeight);
		});
	});
</script>
