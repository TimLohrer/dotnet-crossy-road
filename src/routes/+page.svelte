<script lang="ts">
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
	<GameHud />
	{#if $wsGame.gamePhase == GamePhase.Created}
		<Menu />
	{/if}	
{/if}

<CrossyRoadGame />

{#if $menuState === MenuState.Skins}
	<PopUp>
		<MenuSkins />
	</PopUp>
{:else if $menuState == MenuState.JoinGame}
	<PopUp>
		<MenuMultiplayer />
	</PopUp>
{/if}