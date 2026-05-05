<script lang="ts">
	import Menu from "$lib/Menu.svelte";
	import CrossyRoadGame from "$lib/CrossyRoadGame.svelte";
	import GameHud from "$lib/GameHud.svelte";
	import { MenuState } from "$lib/models/MenuState";
	import { menuState } from "$lib/stores/stateStore";
	import PopUp from "$lib/ PopUp.svelte";
	import { fade } from 'svelte/transition';
	import MenuSkins from "$lib/MenuSkins.svelte";
	import MenuMultiplayer from "$lib/MenuMultiplayer.svelte";


</script>


<Menu />
<CrossyRoadGame />
<GameHud />
{#if $menuState === MenuState.Skins}
	<PopUp>
		<MenuSkins />
	</PopUp>
{:else if $menuState == MenuState.JoinGame}
	<PopUp>
		<MenuMultiplayer />
	</PopUp>
{/if}