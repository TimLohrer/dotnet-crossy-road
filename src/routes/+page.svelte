<script lang="ts">
	import Menu from "$lib/Menu.svelte";
	import CrossyRoadGame from "$lib/CrossyRoadGame.svelte";
	import { user } from "$lib/stores/stateStore";
	import { Skin } from "$lib/models/Skin";
	import GameHud from "$lib/GameHud.svelte";

	$user = {
		id: crypto.randomUUID(),
		username: `Player ${Math.random().toString(36).substring(2, 8)}`,
		highscore: 0,
		taler: 0,
		skin: Skin.Default,
		ownedSkins: [Skin.Default]
	}
</script>

<Menu />
<CrossyRoadGame />
<GameHud />