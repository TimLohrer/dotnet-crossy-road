<div class="container">
    <div class="deathmessage">
        You Died
    </div>
    <button>Restart</button>
</div>

<style>
    .container {
        display: flex;
        align-items: center;
        justify-content: center;
        width: 100%;
        height: 100%;
        position: absolute;
        flex-direction: column;
        z-index: 102;
        backdrop-filter: blur(10px);
        gap: 4rem;
    }

    .deathmessage {
        display: flex;
        justify-content: center;
        align-items: center;
        font-size: 4rem;
        color: red;
        height: 10rem;
        width: 50rem;
        background-color: black;
        border-style: solid;
        border-width: 0.4rem;
        border-color: white;
    }

    button {
        outline: none;
        background-color: rgb(0, 0, 0);
        color: white;
        height: 4rem;
        width: 10rem;
        font-family: PixelFont;
        display: flex;
        justify-content: center;
        align-items: center;
        cursor: pointer;
        border-style: solid;
        border-width: 0.4rem;
        border-color: white;
    }
</style>