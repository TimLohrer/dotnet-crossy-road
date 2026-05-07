<script lang="ts">
	import { MenuState } from "./models/MenuState";
	import { gameSocket, menuState, wsGame } from "./stores/stateStore";

    let seed: number | undefined = $state();

    async function changeSeed() {
        if (seed === undefined) return $menuState = MenuState.Play;
        
        await $gameSocket?.createGame(seed);
    }

</script>
<div class="seed-container">
    <div class="seed-form">
        Seed: <input type="number" name="seed" id="seed" placeholder={$wsGame?.seed.toString()} bind:value={seed}>
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

    input::-webkit-inner-spin-button, input::-webkit-outer-spin-button {
        -webkit-appearance: none;
        margin: 0;
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