<script lang="ts">
	import { PUBLIC_API_URL } from "./environment";
	import SkinPreview from "./SkinPreview.svelte";
	import { gameSocket, menuState, skinList, user } from "./stores/stateStore";
	import type { User } from "./models/User";
	import { SkinRarity, type Skin } from "./models/Skin";
	import { MenuState } from "./models/MenuState";

    async function selectSkin(skin: Skin) {
        const isOwned = $user?.ownedSkins?.some((ownedSkin) => ownedSkin.id === skin.id);
        if (!isOwned) return; 

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
        $gameSocket?.syncPlayerModel();
        $menuState = MenuState.Play;
    }

    async function buySkin(skin: Skin) {
        const isOwned = $user?.ownedSkins?.some((ownedSkin) => ownedSkin.id === skin.id);
        if (isOwned) return selectSkin(skin);

        const skinRes = await fetch(`${PUBLIC_API_URL}/shop/buy`, {
            credentials: 'include',
            method: "POST",
            headers: {
                cookie: document.cookie,
                "Content-Type": "application/json"
            },

            body: JSON.stringify(skin.id)
        })

        if (!skinRes.ok) throw new Error(`failed to select Skin: ${skinRes.status}`);
    
        const newUser = await skinRes.json() as User;
        $user = newUser;
        $gameSocket?.syncPlayerModel();
        $menuState = MenuState.Play;
    }

    let common = "#25D900"
    let rare = "#00E2FC"
    let epic = "#9500FF"
    let legendary = "#F5C100"
    let rarityMap = new Map([[SkinRarity.Common,common],[SkinRarity.Rare,rare],[SkinRarity.Epic,epic],[SkinRarity.Legendary,legendary]]);
</script>

<!-- svelte-ignore component_name_lowercase -->
<div class="container">
    <div class="budget">
        <p>
            <span>Taler:</span>
            <span class="taler">{$user?.taler}</span>
        </p>
        <hr>
    </div>
    <div class="skins">
        {#each $skinList as skin}
            <!-- svelte-ignore a11y_click_events_have_key_events -->
            <!-- svelte-ignore a11y_no_static_element_interactions -->
            <div class="skin" class:owned={$user?.ownedSkins?.some((ownedSkin) => ownedSkin.id === skin.id)} class:selected={$user?.skin?.id === skin.id} onclick={() => selectSkin(skin)} style={$user?.skin?.id === skin.id ? `border-color:${rarityMap.get(skin.rarity) ?? common};box-shadow: 0 0 1rem ${rarityMap.get(skin.rarity) ?? common}` : ''}>
                <div class="preview">
                    <SkinPreview params={{skin: skin}} />
                </div>
                <div class="name">{skin.name}</div>
                {#if !$user?.ownedSkins?.some((ownedSkin) => ownedSkin.id === skin.id)}
                    <div class="pricetag" class:toExpensive={skin.price > ($user?.taler ?? 0)} onclick={() => buySkin(skin)}>
                        {skin.price}
                    </div>
                    <div class="lock">
                        <img src="/assets/lock.png" alt="lock">
                    </div>
                {/if}
            </div>
        {/each}
    </div>
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
        pointer-events: none;
    }

    .skin.owned {
        pointer-events: all;
        cursor: pointer;
    }

    .skins {
        display: flex;
        flex-direction: row;
        flex-wrap: wrap;
        gap: 0.75rem;
        padding: 0.75rem;
    }

    .skin.selected {
        border-width: 0.2rem;
    }

    .container {
        height: 100%;
        width: 33rem;
        background-color: black;
        border-style: none;
        outline: none;
        display: flex;
        flex-direction: column;
        align-content: flex-start;
        gap: 0.75rem;
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
        pointer-events: none;
    }

    .pricetag {
        position: absolute;
        top: 50%;
        left: 50%;
        transform: translate(-50%, -50%);
        z-index: 120;
        padding: 0.3rem 0.6rem;
        border: 0.12rem solid white;
        background-color: rgba(0, 0, 0, 0.8);
        color: white;
        font-weight: 700;
        opacity: 0;
        transition: opacity 0.15s ease-in-out;
        pointer-events: all;
        transition: all .2s ease-in-out;
        cursor: pointer;
    }

    .pricetag:hover {
        border-color: green;
        color: green;
    }

    .pricetag.toExpensive:hover {
        border-color: red;
        color: rgba(255, 0, 0, 1.0);
    }

    .skin:hover .pricetag {
        opacity: 1;
    }

    img {
        width: 1.6rem;
        height: 2rem;
    }

    .budget {
        padding: 0.75rem;
    }

    .budget p {
        display: flex;
        gap: 0.5rem;
        align-items: center;
        justify-content: space-between;
        color: white;
        font-size: 1.1rem;
        font-weight: 600;
    }

    .budget p span.taler {
        color: gold;
    }
</style>