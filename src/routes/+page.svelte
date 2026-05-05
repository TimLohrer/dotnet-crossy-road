<script lang="ts">
	import DeathScreen from './../lib/DeathScreen.svelte';
	import Menu from "$lib/Menu.svelte";
	import CrossyRoadGame from "$lib/CrossyRoadGame.svelte";
	import GameHud from "$lib/GameHud.svelte";
	import { MenuState } from "$lib/models/MenuState";
	import { menuState, wsGame } from "$lib/stores/stateStore";
	import PopUp from "$lib/ PopUp.svelte";
	import MenuSkins from "$lib/MenuSkins.svelte";
	import MenuMultiplayer from "$lib/MenuMultiplayer.svelte";
	import { GamePhase } from "$lib/models/GamePhase";
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