<script lang="ts">
	import { MenuState } from "./models/MenuState";
	import { gameSocket, menuState, wsGame, isDebugMode } from "./stores/stateStore";

    let seed: string | undefined = $state();

    async function changeSeed() {
        if (seed === undefined) return $menuState = MenuState.Play;
        
        if (seed == 'DEBUG') {
            $isDebugMode = !$isDebugMode;
            $gameSocket?.createGame();
            return $menuState = MenuState.Play;
        }

        if (isNaN(Number(seed))) {
            return;
        }

        await $gameSocket?.createGame(Number(seed));
    }
</script>

<div class="seed-container">
    <div class="seed-form">
        Seed: <input type="text" name="seed" id="seed" placeholder={$wsGame?.seed.toString()} bind:value={seed}>
        <button type="button" id="submit" onclick={changeSeed}>Submit</button>
    </div>
</div>

<style>
    .seed-container {
        padding: 1.5rem 2rem;
    }

    input {
        padding: 0 0.5rem;
        outline: none;
        border: none;
    }

    .seed-form {
        display: flex;
        gap: 0.5rem;
        align-items: center;
    }

    #submit {
        cursor: pointer;
        background-color: white;
        border-style: none;
        outline: none;
        padding: 0 0.5rem;
    }

    #submit:hover {
        color: white;
        background-color: black;
    }
</style>