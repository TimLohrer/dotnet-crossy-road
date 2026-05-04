<script lang="ts">
	import favicon from '$lib/assets/favicon.svg';

	const { children, data, params } = $props();

	console.log(data);
</script>

<svelte:head>
	<link rel="icon" href={favicon} />
</svelte:head>
 
{@render children()}
