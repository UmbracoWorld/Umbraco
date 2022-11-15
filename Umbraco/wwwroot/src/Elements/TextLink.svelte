<svelte:options tag="uw-text-link"/>

<script>
    export let href;
    export let target;
</script>

<a href="{href}" target="{target}" class="underline">
    <span>
        <slot/>
    </span>
    <svg viewBox="0 0 13 20">
        <polyline points="0.5 19.5 3 19.5 12.5 10 3 0.5"/>
    </svg>
</a>

<style lang="scss">
  .underline {
    --line:  var(--theme-color);
    --color: var(--theme-color);
    text-decoration: none;
    color: var(--color);
    position: relative;
  }
  .underline span {
    background-image: linear-gradient(0deg, var(--line) 0%, var(--line) 100%);
    background-position: 100% 100%;
    background-repeat: no-repeat;
    background-size: var(--background-size, 100%) 1px;
    transition: background-size 0.2s linear var(--background-delay, 0.15s);
    //font-size: 16px;
    line-height: 20px;
    transform: translateZ(0);
  }
  .underline svg {
    vertical-align: middle;
    display: inline;
    line-height: 1;
    width: 13px;
    height: 1rem;
    position: relative;
    left: 2px;
    fill: none;
    stroke-linecap: round;
    stroke-linejoin: round;
    stroke-width: 3px;
    stroke: var(--line);
    stroke-dasharray: 7.95 30;
    stroke-dashoffset: var(--stroke-dashoffset, 46);
    transition: stroke-dashoffset var(--stroke-duration) var(--stroke-easing, linear) var(--stroke-delay, 0s);
  }
  .underline:hover {
    --background-size: 0%;
    --background-delay: 0s;
    --stroke-dashoffset: 26;
    --stroke-duration: .3s;
    --stroke-easing: cubic-bezier(.3, 1.5, .5, 1);
    --stroke-delay: .195s;
  }

</style>