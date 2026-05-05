<script lang="ts">
	import { gameSocket } from "./stores/stateStore";

    let gameCode = $state("");

    function joinGame(e: SubmitEvent) {
        e.preventDefault();
        $gameSocket?.joinGame(gameCode);
    }
</script>

<div class="container">
    <form class="form" onsubmit={joinGame}>
        <input type="text" name="game" id="gameId" placeholder="GameCode..." bind:value={gameCode}>
        <button type="submit">Join</button>
    </form>
</div>

<style>

    .container {
        font-family: PixelFont;
        background-color: black;
        border-style: none;
        outline: none;
        height: 5rem;
        width: 35rem;
        display: flex;
        justify-content: center;
        align-items: center;
    }

    .form {
        width: 100%;
        height: 100%;
        display: flex;
        flex-direction: row;
        align-items: center;
        justify-content: center;
        gap: 1rem;
    }

    input {
        font-family: PixelFont;
        background-color: rgb(255, 255, 255);
        border-style: none;
        outline: none;
        height: 2rem;
        width: 23rem;
        padding-left: 0.5rem;
        padding-right: 0.5rem;
    }

    button {
        font-family: PixelFont;
        border-style: none;
        outline: none;
        height: 2rem;
        width: 7rem;
        cursor: pointer;
        background-color: white;
        padding-left: 0.5rem;
        padding-right: 0.5rem;
    }

    button:hover {
        background-color: black;
        color: white;
    }
</style>