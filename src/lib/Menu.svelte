<script lang="ts">
	import { PUBLIC_API_URL } from "./environment";
	import { GamePhase } from "./models/GamePhase";
	import { MenuState } from "./models/MenuState";
    import { gameSocket, menuState, user, wsGame } from "./stores/stateStore";

    function changeSeed() {
        $menuState = MenuState.Seed;
    }

    function lookupLeaderboard() {
        $menuState = MenuState.LeaderBoard;
    }
</script>

<div class="logo_container">
    <img src="./assets/CrossyRoadLogo.png" alt="Game Logo" class="Logo">
</div>

{#if $wsGame?.hostId == $user?.id && ($wsGame?.players?.length ?? 0) <= 1 && $wsGame?.gamePhase == GamePhase.Created}
    <div class="seed-bar">
        <button id="seed" onclick={changeSeed}>Change Seed</button>
    </div>
{/if}

{#if $wsGame?.hostId == $user?.id && $wsGame?.gamePhase == GamePhase.Created}
    <div class="leaderboard-bar">
        <button id="leaderboard" onclick={lookupLeaderboard}>LeaderBoard</button>
    </div>
{/if}

<div class="top-bar-wrapper">
    <div class="top-bar">
        {#if $wsGame?.hostId == $user?.id}
            Game Code: <span>{$wsGame?.id.split("-")[0].toUpperCase()}</span>
        {:else}
            Waiting for host to start the game!
        {/if}
    </div>
</div>

<div class="menu_container">
    <div class="menu_panel">
        <button class:active={$menuState == MenuState.Shop} onclick={() => $menuState = MenuState.Shop}>Shop</button>
    </div>
    <div class="menu_panel">
        <button class:active={$menuState == MenuState.Play} onclick={() => $menuState == MenuState.Play && $wsGame?.hostId == $user?.id ? $gameSocket?.startGame() : $menuState = MenuState.Play}>Start</button>
    </div>
    <div class="menu_panel">
        {#if $wsGame && $wsGame.players.length > 1}
            <button onclick={() => $gameSocket?.leaveGame()}>Leave Game</button>
        {:else}
            <button class:active={$menuState == MenuState.JoinGame} onclick={() => $menuState = MenuState.JoinGame}>Multiplayer</button>
        {/if}
    </div>
</div>

<div class="logout">
    <a href={`${PUBLIC_API_URL}/auth/signout?returnUrl=${encodeURIComponent(`${window.location.origin}/signin`)}`}>Sign Out</a>
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
        z-index: 150;
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

    .top-bar-wrapper {
        position: absolute;
        inset: 0;
        display: flex;
        width: 100%;
        align-items: start;
        justify-content: center;
        z-index: 100;
        pointer-events: none;
    }

    .top-bar {
        margin-top: 1rem;
        background-color: black;
        color: white;
        height: 4rem;
        font-size: 1.3rem;
        border-style: solid;
        border-width: 0.4rem;
        border-color: white;
        padding: 0 1rem;
        display: flex;
        align-items: center;
    }

    .seed-bar {
        position: absolute;
        top: 1rem;
        left: 1rem;
        justify-self: center;
        z-index: 100;
        background-color: black;
        color: white;
        height: 4rem;
        font-size: 1.3rem;
        border-style: solid;
        border-width: 0.4rem;
        border-color: white;
        display: flex;
        align-items: center;
        pointer-events: none;
    }

    #seed {
        pointer-events: all;
        width: 100%;
        height: 100%;
        border-style: none;
        padding: 0 1rem;
    }

    #seed:hover {
        color: black;
        background-color: white;
    }
    .leaderboard-bar {
        position: absolute;
        top: 6rem;
        left: 1rem;
        justify-self: center;
        z-index: 100;
        background-color: black;
        color: white;
        height: 4rem;
        font-size: 1.3rem;
        border-style: solid;
        border-width: 0.4rem;
        border-color: white;
        display: flex;
        align-items: center;
        pointer-events: none;
    }

    #leaderboard {
        pointer-events: all;
        width: 100%;
        height: 100%;
        border-style: none;
        padding: 0 1rem;
    }

    #leaderboard:hover {
        color: black;
        background-color: white;
    }

    .top-bar span {
        pointer-events: all;
        margin-left: 0.5rem;
    }

    button {
        width: 20rem;
        height: 7rem;
        cursor: pointer;
        outline: none;
        border-style: none;
        background-color: black;
        color: white;
        font-size: 1.3rem;
        border-style: solid;
        border-width: 0.4rem;
        border-color: white;
    }

    button:hover {
        background-color: white;
        color: black;
    }

    button.active {
        height: 8rem;
    }

    .logout {
        display: flex;
        align-items: center;
        justify-content: center;
        position: absolute;
        top: 1rem;
        right: 1rem;
        z-index: 100;
        width: max-content;
        height: 4rem;
        padding: 0 1rem;
        cursor: pointer;
        border-style: solid;
        border-width: 0.4rem;
        border-color: red;
        backdrop-filter: blur(5px);
        background-color: black;
    }
    
    .logout a {
        font-size: 1rem;
        color: red;
        text-decoration: none;
    }

    .logout:hover {
        background-color: red;
    }

    .logout:hover a {
        color: black;
    }
</style>


