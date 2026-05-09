<script lang="ts">
	import { gameRenderer, gameSocket } from '$lib/stores/stateStore';
	import { onDestroy, onMount } from 'svelte';
	import { GameRenderer } from './GameRenderer';
	import { GameSocket } from './GameSocket';
	import { PUBLIC_API_URL } from './environment';

	let container: HTMLDivElement;
	
	onMount(async () => {
		const otherGameId = window.location.search.split('?').find(param => param.startsWith('gameId='))?.split('=')[1];
		
		$gameRenderer = new GameRenderer(window, container);

		$gameSocket = new GameSocket(`${PUBLIC_API_URL}/game/ws`);
		
		if (otherGameId) {
			$gameSocket!.onReady = async () => {
				await $gameSocket?.joinGame(otherGameId);
			};
		}

		$gameSocket!.connect();

		window.addEventListener('keydown', (e: KeyboardEvent) => $gameRenderer?.onKeyDown(e));
		window.addEventListener('resize', () => $gameRenderer!.resize());
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
		pointer-events: none;
	}
</style>