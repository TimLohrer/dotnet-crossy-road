<script lang="ts">
	import { Game } from "./models/Game";
	import { gameSocket, user, wsGame } from "./stores/stateStore";
</script>

<div class="container">
    <p class="box deathmessage">You Died</p>
    <div class="stats">
        <div class="stat">
            <span>Score</span>
            <span>{Game.getPlayer($wsGame!, $user!.id)?.score}</span>
        </div>
        <div class="stat">
            <span>Taler</span>
            <span>{Game.getPlayer($wsGame!, $user!.id)?.taler}</span>
        </div>
        <div class="stat">
            <span>Time</span>
            <span>{Game.getDuration($wsGame!)}</span>
        </div>
    </div>
    <button class="box" onclick={() => $gameSocket?.createGame()}>Restart</button>
</div>

<style>
    .container {
        display: flex;
        align-items: center;
        justify-content: center;
        width: 100%;
        height: 100%;
        position: absolute;
        flex-direction: column;
        z-index: 102;
        backdrop-filter: blur(10px);
        gap: 4rem;
    }

    .box {
        display: flex;
        border-style: solid;
        border-width: 0.4rem;
        border-color: white;
        background-color: black;
    }

    .deathmessage {
        display: flex;
        justify-content: center;
        align-items: center;
        font-size: 4rem;
        color: red;
        height: 10rem;
        width: 50rem;
        border-style: solid;
        border-width: 0.4rem;
        border-color: white;
    }

    .stats {
        flex-direction: column;
        gap: 1rem;
    }

    .stats .stat {
        display: flex;
        justify-content: space-between;
        width: 30rem;
        font-size: 1.5rem;
        gap: 1rem;
    }

    .stats .stat span:nth-child(2) {
        color: white;
    }

    button {
        outline: none;
        color: white;
        height: 4rem;
        width: 10rem;
        justify-content: center;
        align-items: center;
        cursor: pointer;
        transition: all 100ms;
    }

    button:hover {
        background-color: white;
        color: black;
    }
</style>