<script lang="ts">
	import DeathScreen from './../lib/DeathScreen.svelte';
	import Menu from "$lib/Menu.svelte";
	import CrossyRoadGame from "$lib/CrossyRoadGame.svelte";
	import GameHud from "$lib/GameHud.svelte";
	import { MenuState } from "$lib/models/MenuState";
	import { menuState, wsGame, user as userStore } from "$lib/stores/stateStore";
	import PopUp from "$lib/ PopUp.svelte";
	import MenuSkins from "$lib/MenuSkins.svelte";
	import MenuMultiplayer from "$lib/MenuMultiplayer.svelte";
	import { GamePhase } from "$lib/models/GamePhase";
	import type { User } from '$lib/models/User';
	import { PUBLIC_API_URL } from '$lib/environment';
	import { onMount } from 'svelte';

	onMount(async () => {
		const res = await fetch(`${PUBLIC_API_URL}/user/@me`, {
			credentials: 'include',
			headers: {
				cookie: document.cookie
			}
		});

		if (!res.ok) {
			throw window.location.replace(`/signin`);
		}

		const user = await res.json() as User;

		userStore.update(() => user);
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

{#if $menuState === MenuState.Skins}
	<PopUp params={{ canEscapeToClose: true, onClose: () => $menuState = MenuState.Play }}>
		<MenuSkins />
	</PopUp>
{:else if $menuState == MenuState.JoinGame}
	<PopUp params={{ canEscapeToClose: true, onClose: () => $menuState = MenuState.Play }}>
		<MenuMultiplayer />
	</PopUp>
{/if}