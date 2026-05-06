<script lang="ts">
	import SkinPreview from "./SkinPreview.svelte";
	import { skinList } from "./stores/stateStore";

</script>
<!-- svelte-ignore component_name_lowercase -->
<div class="container">
    {#each $skinList as skin}
        <div class="skin">
            <div class="preview">
                <SkinPreview params={{skin: skin}} />
            </div>
            <div class="name">{skin.name}</div>
        </div>
    {/each}
</div>

<style>
    .skin {
        width: 10rem;
        min-height: 12rem;
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: flex-start;
        gap: 0.5rem;
        cursor: pointer;
        border: 0.12rem solid white;
        padding: 0.5rem;
        box-sizing: border-box;
        background-color: rgba(255, 255, 255, 0.04);
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
        height: 8.5rem;
        overflow: hidden;
    }

    .name {
        width: 100%;
        text-align: center;
        color: white;
        font-size: 0.95rem;
        font-weight: 600;
        line-height: 1.2;
    }
</style>