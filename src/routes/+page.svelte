<script lang="ts">
	import type { Lane } from '$lib/models/Lane';
	import { onMount } from 'svelte';
	import * as THREE from 'three';
	import { GLTFLoader } from 'three/addons/loaders/GLTFLoader.js';
	import type { GLTF, Position } from 'three/examples/jsm/Addons.js';

	const PIXEL = 0.05;
	const gameLoop: boolean = true;
	const objectList: THREE.Object3D[] = [];

	const GLTF_CACHE: { [key: string]: GLTF } = {};

	onMount(async () => {
		document.body.innerHTML = '';
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
			0.01,
			1000
		);
		camera.position.z = -2;
		camera.position.y = 4;
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
			const gltf = GLTF_CACHE[path] ?? await loader.loadAsync(path);
			GLTF_CACHE[path] = gltf;
			let model = gltf.scene;
			model.position.x = pos.x;
			model.position.y = pos.y;
			model.position.z = pos.z;
			scene.add(model);

			if (hasCollision) {
				objectList.push(model);
			}
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
		}

		for (let i = -10; i < 3; i++) {
			if (i < 0) {
				loadModel('/models/default/lanes/plains.gltf', new THREE.Vector3(0, 0, i), false);
			} else {
				loadLane(i);
			}
		}

		const gltf = await loader.loadAsync(`/models/default/elements/tree_0.gltf`);
		const player = gltf.scene as THREE.Object3D;
		player.position.z = -2;
		scene.add(player);

		document.addEventListener('keydown', (event) => {
			const moveDistance = 1;
			switch (event.key.toLowerCase()) {
				case 'w':
					player.position.z += moveDistance;
					break;
				case 'arrowup':
					player.position.z += moveDistance;
					break;
				case 's':
					player.position.z -= moveDistance;
					break;
				case 'arrowdown':
					player.position.z -= moveDistance;
					break;
				case 'a':
					player.position.x += moveDistance;
					break;
				case 'arrowleft':
					player.position.x += moveDistance;
					break;
				case 'd':
					player.position.x -= moveDistance;
					break;
				case 'arrowright':
					player.position.x -= moveDistance;
					break;
			}
			// isPlayerColliding();
		});

		// function isPlayerColliding(): boolean {
		// 	for (const object of objectList) {
		// 		if (object.position.x == player.position.x && object.position.z == player.position.z) {
		// 			scene.clear();
		// 			return true;
		// 		}
		// 	}
		// 	return false;
		// }

		// await loadModel('lanes/plains.gltf', new THREE.Vector3(), false);
		// await loadModel('lanes/water.gltf', new THREE.Vector3(0,0,-1), false);
		// await loadModel('lanes/water.gltf', new THREE.Vector3(0,0,-2), false);
		// await loadModel("elements/log_0.gltf", new THREE.Vector3(-5,-PIXEL,-2), false);
		// await loadModel('elements/stone_1.gltf', new THREE.Vector3(-1, PIXEL, 0), true);
		// await loadModel('elements/tree_1.gltf', new THREE.Vector3(2, PIXEL, 0), true);
		// await loadModel('lanes/street_top.gltf', new THREE.Vector3(0,0,1), false);
		// await loadModel('lanes/street_middle.gltf', new THREE.Vector3(0,0,2), false);
		// await loadModel('lanes/street_bottom.gltf', new THREE.Vector3(0,0,3), false);
		// await loadModel('lanes/plains.gltf', new THREE.Vector3(0,0,4), false);
		// await loadModel('lanes/plains.gltf', new THREE.Vector3(0,0,-3), false);
		// await loadModel('lanes/plains.gltf', new THREE.Vector3(0,0,-4), false);

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
