<script lang="ts">
	import { GamePhase } from "./models/GamePhase";
	import { MenuState } from "./models/MenuState";
    import { menuState, user, wsGame } from "./stores/stateStore";
</script>

<div class="logo_container">
    <img src="./assets/CrossyRoadLogo.webp" alt="Game Logo" class="Logo">
</div>

<div class="menu_container">
    {#if $wsGame?.hostId == $user?.id}
        <div class="menu_panel">
            <button class:active={$menuState == MenuState.Skins} onclick={() => $menuState = MenuState.Skins}>Skins</button>
        </div>
        <div class="menu_panel">
            <button class:active={$menuState == MenuState.Play} onclick={() => $menuState = MenuState.Play}>Start</button>
        </div>
        <div class="menu_panel">
            <button class:active={$menuState == MenuState.JoinGame} onclick={() => $menuState = MenuState.JoinGame}>Multiplayer</button>
        </div>
    {:else}
        <div class="menu_panel">
            <button class="info-panel">Waiting for host to start the game!</button>
        </div>
    {/if}
</div>

<style>
    .menu_container {
        position: absolute;
        inset: 0;
        display: flex;
        flex-direction: row;
        align-self: end;
        align-items: center;
        justify-content: center;
        z-index: 100;
        height: 9rem;
        gap: 1rem;
        pointer-events: all;
    }

    .logo_container {
        position: absolute;
        inset: 0;
        display: flex;
        align-items: center;
        justify-content: center;
        z-index: 100;
        pointer-events: none;
    }

    img {
        width: 40rem;
        padding-bottom: 250px;
    }

    button {
        width: 20rem;
        height: 7rem;
        cursor: pointer;
        outline: none;
        border-style: none;
        background-color: rgb(0, 0, 0);
        color: white;
        font-family: PixelFont;
        font-size: 1.3rem;
        border-style: solid;
        border-width: 0.4rem;
        border-color: white;
    }

    button.active {
        height: 8rem;
    }

    button.info-panel {
        cursor: default;
        width: max-content;
        padding: 0 2rem;
    }
</style>


