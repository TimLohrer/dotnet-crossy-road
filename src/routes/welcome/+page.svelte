<script lang="ts">
	import { PUBLIC_API_URL } from "$lib/environment";
	import type { User } from "$lib/models/User";
	import { user } from "$lib/stores/stateStore";

    let newUsername = $state("");

    function onInput(e: Event) {
        const allowedChars = "@abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_-.";
        const input = e.target as HTMLInputElement;
        newUsername = input.value
            .split("")
            .filter(char => allowedChars.includes(char))
            .join("");
    }

    async function updateUesrname(e: SubmitEvent) {
        e.preventDefault();
        if (newUsername.length < 1) return;
        console.log(JSON.stringify({ username: newUsername }));
        

        const res = await fetch(`${PUBLIC_API_URL}/user/@me`, {
            method: "PATCH",
            credentials: "include",
            headers: {
                "Content-Type": "application/json",
                cookie: document.cookie
            },
            body: JSON.stringify({ username: newUsername })
        });

        if (!res.ok) {
            alert("Failed to update username: " + (await res.text()));
        } else {
            const updatedUser = await res.json() as User;
            user.update(() => updatedUser);
            window.location.replace("/");
        }
    }
</script>

<img src="/assets/bg.webp" alt="background" class="background" />
<div class="root">
    <h1 class="title">Welcome to CrossyRoad!</h1>
    <div class="container">
        <p>Please choose your username:</p>
        <form class="form" onsubmit={updateUesrname}>
            <input type="text" name="username" id="username" placeholder="Your username..." bind:value={newUsername} oninput={onInput}>
            <button type="submit">Play!</button>
        </form>
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
        flex-direction: column;
        justify-content: center;
        align-items: center;
        width: 100%;
        height: 100%;
        backdrop-filter: blur(15px);
    }

    .title {
        position: absolute;
        font-size: 3rem;
        top: 10%;
        text-shadow: .25rem .25rem white;
    }

    .container {
        background-color: black;
        border: .4rem solid white;
        outline: none;
        height: max-content;
        width: 35rem;
        padding: 1rem;
        display: flex;
        flex-direction: column;
        justify-content: center;
        align-items: center;
    }

    .container p {
        place-self: flex-start;
        font-size: .65rem;
        color: white;
    }

    .form {
        width: 100%;
        height: 100%;
        display: flex;
        flex-direction: row;
        align-items: center;
        justify-content: center;
        gap: 1rem;
    }

    input {
        font-family: PixelFont;
        background-color: rgb(255, 255, 255);
        border-style: none;
        outline: none;
        height: 2rem;
        width: 100%;
        padding: 0 0.5rem;
    }

    button {
        font-family: PixelFont;
        border-style: none;
        outline: none;
        height: 2rem;
        width: max-content;
        padding: 0 .5rem;
        cursor: pointer;
        background-color: white;
        transition: all 100ms;
    }

    button:hover {
        background-color: black;
        color: white;
    }
</style>