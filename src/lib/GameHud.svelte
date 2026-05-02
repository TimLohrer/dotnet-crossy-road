<script lang="ts">
	import type { Player } from "./models/Player";
	import { user, wsGame } from "./stores/stateStore";

    let player: Player | undefined = $state();
    wsGame.subscribe(game => {
        player = game?.players.find(p => p.user.id === $user?.id);
    });
</script>

{#if player}
    <div class="hud">
        <h1>Score: {player.score}</h1>
    </div>
{/if}

<style>
    .hud {
        position: absolute;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        padding: 1rem;
        pointer-events: none;
    }
</style>