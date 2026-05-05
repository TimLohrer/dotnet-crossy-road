<script lang="ts">
	import favicon from '$lib/assets/favicon.svg';
	import { PUBLIC_API_URL } from '$lib/environment.js';
	import type { User } from '$lib/models/User.js';
	import { user as userStore } from '$lib/stores/stateStore.js';
	import { onMount } from 'svelte';

	const { children } = $props();

	onMount(async () => {
		const res = await fetch(`${PUBLIC_API_URL}/user/@me`, {
			credentials: 'include',
			headers: {
				cookie: document.cookie
			}
		});

		if (!res.ok) {
			throw window.location.replace(`${PUBLIC_API_URL}/auth/signin/bosch`);
		}

		const user = await res.json() as User;

		userStore.set(user);
	});
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

	:global(body) {
		font-family: PixelFont;
	}
</style>
