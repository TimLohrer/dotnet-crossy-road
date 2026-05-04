<script lang="ts">
	import { GamePhase } from "./models/GamePhase";
	import { user, wsGame } from "./stores/stateStore";
</script>

{#if $wsGame}
    <div class="hud">
        <h1>Score: {$wsGame?.players.find(p => p.user.id === $user?.id)?.score} Seed: {$wsGame?.seed} Phase: {$wsGame.gamePhase}</h1>
        {#if $wsGame.gamePhase == GamePhase.Created}
            <button onclick={() => window.open(`/?gameId=${$wsGame.id}`)}>Join as new player</button>
        {/if}
    </div>
{/if}

<style>
    .hud {
        display: flex;
        flex-direction: row;
        position: absolute;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        gap: 1rem;
        padding: 1rem;
        z-index: 1000;
        pointer-events: none;
    }

    .hud > button {
        pointer-events: all;
        height: min-content;
    }
</style>