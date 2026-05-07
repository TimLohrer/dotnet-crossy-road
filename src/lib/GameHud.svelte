<script lang="ts">
	import { user, wsGame } from "./stores/stateStore";
</script>

<div class="hud">
    <p>{$wsGame?.players.find(p => p.user.id === $user?.id)?.score}</p>
    <p class="taler">{($user?.taler ?? 0) + ($wsGame?.players.find(p => p.user.id === $user?.id)?.taler ?? 0)}</p>
</div>

<style>
    .hud {
        display: flex;
        flex-direction: row;
        justify-content: space-between;
        width: calc(100% - 2rem);
        position: absolute;
        top: 1rem;
        left: 1rem;
        right: 1rem;
        gap: 1rem;
        padding: 1rem;
        z-index: 1000;
    }
    
    p {
        background-color: black;
        color: white;
        max-height: min-content;
        font-size: 1.3rem;
        border-style: solid;
        border-width: 0.4rem;
        border-color: white;
        padding: 0 2rem;
        display: flex;
        align-items: center;
        font-size: 2.5rem;
    }

    .taler {
        border-color: gold;
        color: gold;
    }
</style>