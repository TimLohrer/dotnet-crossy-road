<div class="container">
    <div class="popup"></div>
</div>

<style>
    .container {
        display: flex;
        justify-content: center;
        align-items: center;
        width: 100%;
        height: 100%;
        z-index: 100;
        position: absolute;
        backdrop-filter: blur(0px);
        pointer-events: none;
    }

    .popup {
        width: 40rem;
        height: 45rem;
        background-color: rgb(0, 0, 0);
        box-shadow: rgb(255, 255, 255) 7px 7px 0px;   
        margin-bottom: 7rem;
        pointer-events: all;
    }

</style>