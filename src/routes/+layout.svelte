<script lang="ts">
	import favicon from '$lib/assets/favicon.ico';
	import { PUBLIC_API_URL } from '$lib/environment';
	import type { User } from '$lib/models/User';
	import { onMount } from 'svelte';
	import { user as userStore } from '$lib/stores/stateStore';

	const { children } = $props();

	onMount(async () => {
		if (window.location.pathname === '/signin' || window.location.pathname === '/welcome') {
			return;
		}
		
		const res = await fetch(`${PUBLIC_API_URL}/user/@me`, {
			credentials: 'include',
			headers: {
				cookie: document.cookie
			}
		});

		if (!res.ok) {
			throw window.location.replace(`/signin?returnUrl=${encodeURIComponent(window.location.origin + window.location.pathname)}`);
		}

		const user = await res.json() as User;
		userStore.update(() => user);

		if (user.username.endsWith("-CHANGE_ME")) {
			window.location.replace("/welcome");
		}
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

	:global(*) {
		font-family: PixelFont !important;
		margin: 0;
		padding: 0;
		box-sizing: border-box;
	}
</style>
