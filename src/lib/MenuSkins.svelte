<script>
	import { div } from "three/tsl";
	import { Skin } from "./models/Skin";
	import SkinPreview from "./SkinPreview.svelte";

</script>
<!-- svelte-ignore component_name_lowercase -->
<div class="container">
    {#each Object.keys(Skin).filter(key => isNaN(Number(key))) as skin}
        <div class="skin">
            <div class="preview">
                <SkinPreview />
            </div>
            {skin}
        </div>
    {/each}
</div>

<style>
    .skin {
        background-color: aqua;
        width: 10rem;
        height: 12rem;
    }

    .container {
        height: 45rem;
        width: 40rem;
        background-color: black;
        border-style: none;
        outline: none;
    }
</style>