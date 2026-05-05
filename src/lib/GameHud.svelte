<script lang="ts">
	import { GamePhase } from "./models/GamePhase";
	import { user, wsGame } from "./stores/stateStore";
</script>

<div class="hud">
    <p>Score: {$wsGame?.players.find(p => p.user.id === $user?.id)?.score} Seed: {$wsGame?.seed} Phase: {$wsGame?.gamePhase}</p>
</div>

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
</style>