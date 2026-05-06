<script lang="ts">
	import { PUBLIC_API_URL } from "./environment";
	import SkinPreview from "./SkinPreview.svelte";
	import { gameSocket, skinList, user } from "./stores/stateStore";
	import type { User } from "./models/User";
	import type { Skin } from "./models/Skin";

    async function handleEvent(skin: Skin) {
        const skinRes = await fetch(`${PUBLIC_API_URL}/shop/select`, {
			credentials: 'include',
            method: "POST",
			headers: {
				cookie: document.cookie,
                "Content-Type": "application/json"
			},

            body: JSON.stringify(skin.id)
		});

        if (!skinRes.ok) throw new Error(`failed to select Skin: ${skinRes.status}`);

        const newUser = await skinRes.json() as User;
        $user = newUser;
        $gameSocket?.syncPlayerModel()
    }

</script>
<!-- svelte-ignore component_name_lowercase -->
<div class="container">
    {#each $skinList as skin}
        <!-- svelte-ignore a11y_click_events_have_key_events -->
        <!-- svelte-ignore a11y_no_static_element_interactions -->
        <div class="skin" onclick={() => handleEvent(skin)}>
            <div class="preview">
                <SkinPreview params={{skin: skin}} />
            </div>
            <div class="name">{skin.name}</div>
            {#if !$user?.ownedSkins?.some((ownedSkin) => ownedSkin.id === skin.id)}
                <div class="lock">
                    <img src="/assets/lock.png" alt="lock">
                </div>
            {/if}
        </div>
    {/each}
</div>



<style>
    .skin {
        width: 10rem;
        height: 12rem;
        position: relative;
        display: flex;
        flex-direction: column;
        align-items: stretch;
        justify-content: flex-start;
        cursor: pointer;
        border: 0.12rem solid white;
        box-sizing: border-box;
        background-color: rgba(255, 255, 255, 0.04);
        overflow: hidden;
    }

    .container {
        height: 45rem;
        width: 40rem;
        background-color: black;
        border-style: none;
        outline: none;
        display: flex;
        flex-direction: row;
        flex-wrap: wrap;
        align-content: flex-start;
        gap: 0.75rem;
        padding: 0.75rem;
        box-sizing: border-box;
        overflow-y: auto;
    }

    .preview {
        width: 100%;
        flex: 1 1 auto;
        min-height: 0;
        overflow: hidden;
    }

    .name {
        width: 100%;
        padding: 0.5rem;
        box-sizing: border-box;
        text-align: center;
        color: white;
        font-size: 0.95rem;
        font-weight: 600;
        line-height: 1.2;
        z-index: 101;
        border-color: white;
        border-width: 0.12rem;
        border-style: solid;
    }

    .lock {
        position: absolute;
        inset: 0;
        display: flex;
        justify-content: end;
        align-items: start;
        padding: 0.5rem;
        background-color: rgba(0, 0, 0, 0.5);
        backdrop-filter: blur(2px);
        backdrop-filter: grayscale(100%);
        pointer-events: none;
    }

    img {
        width: 1.6rem;
        height: 2rem;
    }
</style>