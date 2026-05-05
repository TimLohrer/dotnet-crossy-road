<script lang="ts">
    import type { TransitionConfig } from "svelte/transition";
    const { children } = $props();

    function popupIn(_node: Element, { duration = 200, blur = 10 } = {}): TransitionConfig {
        return {
            duration,
            css: (t) => `
                opacity: ${t};
                backdrop-filter: blur(${t * blur}px);
                -webkit-backdrop-filter: blur(${t * blur}px);
            `
        };
    }
</script>

<div class="container" in:popupIn out:popupIn>
    <div class="popup">
        {@render children()}
    </div>
</div>

<style>

    .container {
        display: flex;
        justify-content: center;
        align-items: center;
        width: 100%;
        height: 100%;
        z-index: 100;
        position: absolute;
        pointer-events: none;
        opacity: 1;
        backdrop-filter: blur(10px);
        -webkit-backdrop-filter: blur(10px);
    }

    .popup {
        min-width: max-content;
        min-height: max-content;
        background-color: rgb(0, 0, 0);
        margin-bottom: 7rem;
        pointer-events: all;
        color: white;
        border-style: solid;
        border-width: 0.4rem;
        border-color: white;
    }

</style>