<script lang="ts">
	import { PUBLIC_API_URL } from '$lib/environment';

    const methods = [
        {
            name: 'Microsoft',
            image: '/assets/microsoft.webp',
            method: 'microsoft',
            fontColor: '#000000',
            color: '#FFFFFF'
        }
    ];

    let returnUrl: string | null = $state(null);

     $effect(() => { returnUrl = window.location.origin; });
</script>

<img src="/assets/bg.webp" alt="background" class="background" />
<div class="root">
    <div class="container">
        <h1>Sign In</h1>
        {#each methods as method}
            <a href={`${PUBLIC_API_URL}/auth/signin/${method.method}?returnUrl=${returnUrl}`} class="sign-in-button" style="background-color: {method.color}; color: {method.fontColor}">
                {#if method.image}
                    <img src={method.image} alt={method.name} class="sign-in-image" />
                {/if}
                <span>{method.name}</span>
            </a>
        {/each}
    </div>
</div>

<style>
    .background {
        position: absolute;
        width: 100%;
        height: 100%;
        object-fit: cover;
        z-index: -1;
    }

    .root {
        display: flex;
        justify-content: center;
        align-items: center;
        width: 100%;
        height: 100%;
        backdrop-filter: blur(15px);
    }

    .container {
        display: flex;
        flex-direction: column;
        align-items: center;
        background-color: black;
        padding: 2.5rem;
        min-width: 30rem;
        border: 0.4rem solid white;
    }

    h1 {
        font-size: 1.5rem;
        margin-bottom: .5rem;
        color: white;
    }

    .sign-in-button {
        display: flex;
        align-items: center;
        justify-content: center;
        width: 100%;
        padding: 1rem 2rem;
        text-decoration: none;
        margin-top: 1rem;
        font-size: .75rem;
        gap: 1rem;
        height: 3.5rem;
    }

    .sign-in-image {
        height: 1.8rem;
    }

    @media (max-width: 600px) {
        .container {
            min-width: 80%;
        }

        .sign-in-button {
            padding: 0.75rem 1.5rem;
        }

        .sign-in-image {
            display: none;
        }
    }
</style>