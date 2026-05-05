<script lang="ts">
    import type { TransitionConfig } from "svelte/transition";
	import { onMount } from "svelte";

    interface PopUpProps {
        children: () => any;
        params?: {
            canEscapeToClose?: boolean;
            onClose?: () => void;
        }
    }

    const { children, params }: PopUpProps = $props();

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

    onMount(() => {
        if (params?.canEscapeToClose) {
            const handleKeyDown = (e: KeyboardEvent) => {
                if (e.key.toLowerCase() === "escape") {
                    params.onClose?.();
                }
            };
            window.addEventListener("keydown", handleKeyDown);
            return () => window.removeEventListener("keydown", handleKeyDown);
        }
    });
</script>

<!-- svelte-ignore a11y_click_events_have_key_events -->
<!-- svelte-ignore a11y_no_static_element_interactions -->
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
        opacity: 1;
        backdrop-filter: blur(10px);
        -webkit-backdrop-filter: blur(10px);
    }

    .popup {
        min-width: max-content;
        min-height: max-content;
        background-color: black;
        margin-bottom: 7rem;
        pointer-events: all;
        color: white;
        border-style: solid;
        border-width: 0.4rem;
        border-color: white;
    }

</style>