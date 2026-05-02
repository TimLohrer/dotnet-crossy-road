<script lang="ts">
	import { isPlaying, gameState, user, wsGame, gameRenderer, gameSocket } from '$lib/stores/stateStore';
	import { onDestroy, onMount } from 'svelte';
	import { GameState } from './models/GameState';
	import { GameRenderer } from './GameRenderer';
	import { Game } from './models/Game';
	import { GameSocket } from './GameSocket';

	let container: HTMLDivElement;
	
	onMount(async () => {
		const otherGameId = window.location.search.split('?').find(param => param.startsWith('gameId='))?.split('=')[1];
		
		$gameRenderer = new GameRenderer(window, container);

		$gameSocket = new GameSocket("http://localhost:5016/api/v1/game/ws");
		
		if (otherGameId) {
			$gameSocket!.onReady = async () => {
				await $gameSocket?.joinGame(otherGameId);
			};
		}

		$gameSocket!.connect();

		document.addEventListener('keyup', handleKeyUp);
		async function handleKeyUp(event: KeyboardEvent) {
			const player = Game.getPlayer($wsGame!, $user!.id)!;
			const moveDistance = 1;
			let newPosition = player.position.clone();
			if ($gameState != GameState.Skins) {
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
					default:
						return;
				}
				$isPlaying = true; 

				if (!$gameRenderer!.isPlayerColliding(newPosition.toVector3())) {
					player.position = newPosition;
					$gameRenderer!.updatePlayerPosition(player);
				}
			}
		}
	});

	onDestroy(async () => {
		await $gameSocket?.destroy();
	});
</script>

<div bind:this={container} class="three"></div>

<style>
	.three {
		position: absolute;
		inset: 0;
		z-index: 0; /* background */
	}
</style>