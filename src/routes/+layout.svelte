<script lang="ts">
	import favicon from '$lib/assets/favicon.ico';

	const { children } = $props();
</script>

<svelte:head>
	<link rel="icon" href={favicon} />
</svelte:head>

{@render children()}

<style>
	@font-face {
        font-family: "PixelFont";
        src: url("/game-over-fireball760-fonts/game-over.otf");
        font-weight: normal;
        font-style: normal;
    }

	:global(*) {
		font-family: PixelFont !important;
		margin: 0;
		padding: 0;
		box-sizing: border-box;
	}
</style>
