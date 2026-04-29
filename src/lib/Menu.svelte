<script lang="ts">
    import { isPlaying } from "./stores/gameStore";
    import { type Snippet } from "svelte";

    interface Props {
        children?: Snippet;
    }

    let { children }: Props = $props();

    function startGame() {
        isPlaying.set(true);
    }
</script>

{#if !$isPlaying}
    <div class="menu_container">
        <!-- svelte-ignore slot_element_deprecated -->
        <nav class="menu">
            {@render children?.()}
            <button onclick={() => isPlaying.set(true)}>Start</button>
            <button>options</button>
            <button>quit</button>
        </nav>
    </div>
{/if}

<style>

    .menu_container {
        position: absolute;
        inset: 0;
        display: flex;
        align-items: center;
        justify-content: center;
        z-index: 10;
    }

    .menu {
        width: 20rem;
        height: 30rem;
        background-color: rgba(1,1,1,0.5);
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
        border-radius: 1rem;
        gap: 3rem;
        box-shadow: rgba(0, 0, 0, 0.35) 0px 5px 15px;
    }

    button {
        width: 10rem;
        height: 3rem;
        border-radius: 1rem;
        cursor: pointer;
        outline: none;
        border-style: none;
        background-color: rgb(255, 0, 255);
    }

    button:hover {
        background-color: aquamarine;
    }

</style>


