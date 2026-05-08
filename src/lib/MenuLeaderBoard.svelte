<script lang="ts">
    import { onMount } from "svelte";
	import { PUBLIC_API_URL } from "./environment";
	import type { UserMinimal } from "./models/UserMinimal";
	import { SkinRarity } from "./models/Skin";

    let leaderBoard: UserMinimal[] = $state([]);

    async function getLeaderBoard() {
        const res = await fetch(`${PUBLIC_API_URL}/leaderboard`,{
            credentials: 'include',
            headers: {
                cookie: document.cookie,
                "Content-Type": "application/json"
            }
        })

        if (!res.ok) throw new Error(`failed to fetch Leaderboard: ${res.status}`);

        leaderBoard = await res.json() as UserMinimal[];

        if (leaderBoard.length < 10) {
            const leaderBoardLength = leaderBoard.length;
            for (let i = 0; i < 10 - leaderBoardLength; i++) {
                leaderBoard.push({id: "?", username: "???", highScore: 0, skin: {id:0, name: "", modelName: "", price: 0, rarity: SkinRarity.Common}})
            }
        }
    }

    onMount(() => {
        void getLeaderBoard();
    });

</script>
<div class="container">
    {#each leaderBoard as user, i}
        <div class="leaderboard">
            <span class="rank">#{i + 1}</span>
            <span class="name">{user.username}</span>
            <span class="score">{user.highScore}</span>
        </div>
    {/each}
</div>

<style>
    .container {
        padding: 1.5rem 2rem;
        z-index: 101;
        width: 24rem;
    }

    .leaderboard {
        display: grid;
        grid-template-columns: 3.5rem 1fr auto;
        align-items: center;
        gap: 0.75rem;
        padding: 0.5rem 0;
    }

    .rank {
        font-weight: 700;
    }

    .leaderboard:nth-child(1) .rank {
        color: gold;
    }

    .leaderboard:nth-child(2) .rank {
        color: silver;
    }

    .leaderboard:nth-child(3) .rank {
        color: rgb(138, 69, 23);
    }

    .score {
        font-weight: 700;
    }
</style>