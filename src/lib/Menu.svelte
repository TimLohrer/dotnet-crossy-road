<script lang="ts">
	import { GamePhase } from "./models/GamePhase";
	import { MenuState } from "./models/MenuState";
    import { menuState, wsGame } from "./stores/stateStore";
    import { type Snippet } from "svelte";

    interface Props {
        children?: Snippet;
    }

    let { children }: Props = $props();

    let activeButton = $state("START");

</script>

{#if $wsGame?.gamePhase == GamePhase.Created}
    <div class="logo_container">
        {@render children?.()}
        <img src="./assets/CrossyRoadLogo.webp" alt="Game Logo" class="Logo">
    </div>

    <div class="menu_container">
        <nav class="menu">
            <div class="menu_panel">
                <button class:active={activeButton=="SKINS"} onclick={() => {
                    activeButton = "SKINS";
                    $menuState = MenuState.Skins;
                }}>Skins</button>
            </div>
            <div class="menu_panel">
                <button class:active={activeButton=="START"} onclick={() => {
                    activeButton = "START";
                    $menuState = MenuState.Singleplayer;
                }}>Start</button>
            </div>
            <div class="menu_panel">
                <button class:active={activeButton=="MULTIPLAYER"} onclick={() => {
                    activeButton = "MULTIPLAYER";
                    $menuState = MenuState.Multiplayer;
                }}>Multiplayer</button>
            </div>
        </nav>
    </div>

{/if}

<style>
    * {
        padding: 0;
        margin: 0;
        box-sizing: border-box;
    }

    @font-face {
        font-family: "PixelFont";
        src: url("/game-over-fireball760-fonts/game-over.otf");
        font-weight: normal;
        font-style: normal;
    }
    
    .menu_container {
        position: absolute;
        inset: 0;
        display: flex;
        flex-direction: row;
        align-items: end;
        justify-content: center;
        z-index: 10;
    }

    .logo_container {
        position: absolute;
        inset: 0;
        display: flex;
        align-items: center;
        justify-content: center;
        z-index: 10;
    }

    img {
        width: 40rem;
        padding-bottom: 250px;
    }

    .menu {
        display: flex;
        flex-direction: row;
        align-items: center;
        justify-content: end;
        height: 9rem;
        gap: 1rem;
    }

    button {
        width: 20rem;
        height: 7rem;
        cursor: pointer;
        outline: none;
        border-style: none;
        background-color: rgb(0, 0, 0);
        color: white;
        border-width: 0.4rem;
        font-family: PixelFont;
        font-size: 30;
        border-style: solid;
        border-width: 7px;
        border-color: white;
    }

    button.active {
        height: 8rem;
    }

</style>


