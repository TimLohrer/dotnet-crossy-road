<script lang="ts">
    import * as THREE from 'three';
    import { GLTFLoader, type GLTF } from 'three/examples/jsm/Addons.js';
    import { onMount, onDestroy } from 'svelte';
    import type { Skin } from './models/Skin';

    interface Props {
        params: {
            skin: Skin;
        };
    }

    let { params }: Props = $props();

    const GLTF_CACHE: { [key: string]: GLTF } = {};
    const gltfLoader = new GLTFLoader();

    let container: HTMLDivElement;
    let renderer: THREE.WebGLRenderer | null;
    let scene: THREE.Scene | null;
    let camera: THREE.PerspectiveCamera | null;
    let mixers: THREE.AnimationMixer[] = [];
    let rotatingModel: THREE.Object3D | null = null;
    let animationFrameId: number | null = null;
    let disposed = false;

    const disposeSceneNode = (object: THREE.Object3D) => {
        const mesh = object as THREE.Mesh;
        if (mesh.geometry) {
            mesh.geometry.dispose();
        }

        if (mesh.material) {
            if (Array.isArray(mesh.material)) {
                mesh.material.forEach((material) => material.dispose());
            } else {
                mesh.material.dispose();
            }
        }
    };

    onMount(async () => {
        const width = container.clientWidth;
        const height = container.clientHeight;

        scene = new THREE.Scene();

        // Camera setup
        const aspect = width / Math.max(height, 1);
        camera = new THREE.PerspectiveCamera(40, aspect, 0.001, 1000);
        camera.position.set(-1.7, 1.2, -1.5);
        camera.lookAt(0, 0.8, 0);

        renderer = new THREE.WebGLRenderer({ antialias: false, alpha: true });
        renderer.setSize(width, height);
        renderer.shadowMap.enabled = true;
        renderer.shadowMap.type = THREE.PCFShadowMap;
        container.appendChild(renderer.domElement);

        const ambLight = new THREE.AmbientLight(0xffffff, 0.3);
        scene.add(ambLight);

        const dirLight = new THREE.DirectionalLight(0xffffff, 2);
        dirLight.castShadow = true;
        dirLight.position.set(20, 55, -3);
        dirLight.shadow.normalBias = 0.05;
        dirLight.shadow.camera.top = 18;
        dirLight.shadow.camera.bottom = -18;
        dirLight.shadow.camera.left = -18;
        dirLight.shadow.camera.right = 18;
        dirLight.shadow.camera.near = 0.1;
        dirLight.shadow.camera.far = 100;
        dirLight.shadow.mapSize.width = 2048;
        dirLight.shadow.mapSize.height = 2048;
        dirLight.shadow.intensity = 0.8;
        scene.add(dirLight);
        scene.add(dirLight.target);

        async function loadModel(path: string) {
            const fileName = path.split('/').pop() as string;
            const gltf =
                GLTF_CACHE[fileName] ?? (await gltfLoader.loadAsync(path));
            if (!GLTF_CACHE[fileName]) {
                GLTF_CACHE[fileName] = gltf;
            }
            const model = gltf.scene.clone() as THREE.Object3D;

            model.traverse((child) => {
                if (child.isObject3D) {
                    child.castShadow = true;
                    child.receiveShadow = true;
                }
            });

            if (!scene) return;
            scene.add(model);
            rotatingModel = model;

            const idleAnimation = gltf.animations.find((a) => a.name.toLowerCase() === 'idle');
            if (idleAnimation) {
                const mixer = new THREE.AnimationMixer(model);
                mixer.clipAction(idleAnimation).play();
                mixers.push(mixer);
            }
        }

        await loadModel(`/models/skins/${params.skin.modelName}.gltf`);

        const timer = new THREE.Timer();
        const animate = () => {
            if (disposed || !renderer || !scene || !camera) return;
            animationFrameId = requestAnimationFrame(animate);

            timer.update();
            const deltaTime = timer.getDelta();
            mixers.forEach((mixer) => mixer.update(deltaTime));
            if (rotatingModel) {
                rotatingModel.rotation.y += deltaTime * 1.25;
            }
            renderer.render(scene, camera);
        };

        animate();
    });

    onDestroy(() => {
        disposed = true;

        if (animationFrameId !== null) {
            cancelAnimationFrame(animationFrameId);
        }
        animationFrameId = null;

        mixers.forEach((mixer) => {
            mixer.stopAllAction();
            mixer.uncacheRoot(mixer.getRoot());
        });
        mixers = [];

        if (scene) {
            scene.traverse(disposeSceneNode);
        }

        if (renderer) {
            renderer.dispose();
            renderer.forceContextLoss();
            renderer.domElement.remove();
        }
        renderer = null;
        scene = null;
        camera = null;
        rotatingModel = null;
    });
</script>

<div bind:this={container} class="skin"></div>

<style>
    .skin {
        width: 100%;
        height: 100%;
    }
</style>