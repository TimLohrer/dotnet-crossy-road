<script lang="ts">
	import DeathScreen from './../lib/DeathScreen.svelte';
	import Menu from "$lib/Menu.svelte";
	import CrossyRoadGame from "$lib/CrossyRoadGame.svelte";
	import GameHud from "$lib/GameHud.svelte";
	import { MenuState } from "$lib/models/MenuState";
	import { menuState, wsGame, skinList } from "$lib/stores/stateStore";
	import PopUp from "$lib/ PopUp.svelte";
	import MenuSkins from "$lib/MenuShop.svelte";
	import MenuMultiplayer from "$lib/MenuMultiplayer.svelte";
	import { GamePhase } from "$lib/models/GamePhase";
	import { PUBLIC_API_URL } from '$lib/environment';
	import { onMount } from 'svelte';
	import type { Skin } from '$lib/models/Skin';
	import MenuSeed from '$lib/MenuSeed.svelte';
	import MenuLeaderBoard from '$lib/MenuLeaderBoard.svelte';

	onMount(async () => {
		const skinRes = await fetch(`${PUBLIC_API_URL}/shop/skins`, {
			credentials: 'include',
			headers: {
				cookie: document.cookie
			}
		});

		const skins = await skinRes.json() as Skin[]; 
		if (!skinRes.ok) {
			throw new Error('Failed to fetch skins');
		}
		skinList.update(() => skins); 
	});
</script>


{#if $wsGame}
	{#if $wsGame.gamePhase == GamePhase.Created}
		<Menu />
	{:else if $wsGame.gamePhase == GamePhase.Active}
		<GameHud />
	{:else if $wsGame.gamePhase == GamePhase.Ended}
		<DeathScreen />
	{/if}
{/if}

<CrossyRoadGame />

{#if $menuState === MenuState.Shop}
	<PopUp params={{ canEscapeToClose: true, onClose: () => $menuState = MenuState.Play }}>
		<MenuSkins />
	</PopUp>
{:else if $menuState == MenuState.JoinGame}
	<PopUp params={{ canEscapeToClose: true, onClose: () => $menuState = MenuState.Play }}>
		<MenuMultiplayer />
	</PopUp>
{:else if $menuState == MenuState.Seed}
	<PopUp params={{ canEscapeToClose: true, onClose: () => $menuState = MenuState.Play }}>
		<MenuSeed />
	</PopUp>
{:else if $menuState == MenuState.Leaderboard}
	<PopUp params={{ canEscapeToClose: true, onClose: () => $menuState = MenuState.Play }}>
		<MenuLeaderBoard />
	</PopUp>
{/if}
