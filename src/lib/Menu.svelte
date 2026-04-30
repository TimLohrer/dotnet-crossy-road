<script lang="ts">
	import { GameState } from "./models/GameState";
    import { gameState, isPlaying } from "./stores/gameStore";
    import { onMount, type Snippet } from "svelte";

    interface Props {
        children?: Snippet;
    }

    let { children }: Props = $props();

    function startGame() {
        isPlaying.set(true);
    }

    let activeButton = $state()

    activeButton = "START"

</script>

{#if !$isPlaying}
    <div class="logo_container">
            {@render children?.()}
            <img src="./assets/CrossyRoadLogo.webp" alt="Game Logo" class="Logo">
    </div>

    <div class="menu_container">
        <nav class="menu">
            <div class="menu_panel">
                <button class:active={activeButton=="SKINS"} onclick={() => {
                    activeButton = "SKINS";
                    $gameState = GameState.Skins;
                }}>Skins</button>
            </div>
            <div class="menu_panel">
                <button class:active={activeButton=="START"} onclick={() => {
                    activeButton = "START";
                    $gameState = GameState.Singleplayer;
                    
                }}>Start</button>
            </div>
            <div class="menu_panel">
                <button class:active={activeButton=="MULTIPLAYER"} onclick={() => {
                    activeButton = "MULTIPLAYER";
                    $gameState = GameState.Multiplayer;
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


