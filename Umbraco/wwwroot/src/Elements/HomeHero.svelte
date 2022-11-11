<svelte:options tag="uw-home-hero"/>
<script>
    import {onMount} from 'svelte';

    let root;
    let planetLarge;
    let planetSmall;

    const TRIES_PER_BOX = 50;
    const randUint = range => Math.random() * range | 0;

    function randomIntFromInterval(min, max) {
        return Math.floor(Math.random() * (max - min + 1) + min)
    }

    onMount(() => {
        const placing = [...root.querySelectorAll(".star")].map(el => Bounds(el, 5));
        const fitted = [];
        const areaToFit = Bounds();

        let maxTries = TRIES_PER_BOX * placing.length;
        while (placing.length && maxTries > 0) {
            let i = 0;
            while (i < placing.length) {
                const box = placing[i];
                box.moveTo(randUint(areaToFit.w - box.w), randUint(areaToFit.h - box.h));
                if (fitted.every(placed => !placed.overlaps(box))) {
                    fitted.push(placing.splice(i--, 1)[0].placeElement());
                } else {
                    maxTries--
                }
                i++;
            }
        }

        function Bounds(el, pad = 0) {
            const box = el?.getBoundingClientRect() ?? {
                left: 0, top: 0,
                right: innerWidth, bottom: innerHeight,
                width: innerWidth, height: innerHeight
            };
            return {
                l: box.left - pad,
                t: box.top - pad,
                r: box.right + pad,
                b: box.bottom + pad,
                w: box.width + pad * 2,
                h: box.height + pad * 2,
                overlaps(bounds) {
                    return !(
                        this.l > bounds.r ||
                        this.r < bounds.l ||
                        this.t > bounds.b ||
                        this.b < bounds.t
                    );
                },
                moveTo(x, y) {
                    this.r = (this.l = x) + this.w;
                    this.b = (this.t = y) + this.h;
                    return this;
                },
                placeElement() {
                    if (el) {
                        el.style.top = (this.t + pad) + "px";
                        el.style.left = (this.l + pad) + "px";
                        el.style.width = randomIntFromInterval(12, 30) + "px";
                        el.style.setProperty('--star-rotate-deg', randomIntFromInterval(0, 180) + "deg");
                        el.classList.add("placed");
                    }
                    return this;
                }
            };
        }
    });
</script>
<div class="home-hero" bind:this={root}>

    <img bind:this={planetLarge} alt="Large peach planet" class="planet--large" src="/assets/images/planetLarge.svg"/>


    <img bind:this={planetSmall} alt="Small blue planet with moon" class="planet--small"
         src="/assets/images/planetSmall.svg"/>

    <img src="/assets/images/world.png" class="planet--brand" alt="Umbraco World logo" width="150" height="150"/>


    {#each Array(15) as _, i}
        <img src="/assets/images/Star.svg" class="star" alt="SVG Star"/>
    {/each}
    <div class="home-hero--inner">
        <h1 class="home-hero--heading ">
            <slot name="heading"/>
        </h1>
        <p>
            <slot name="subheading"/>
        </p>
    </div>
</div>


<style lang="scss">
  @import "../styles/base/_functions";

  .home-hero {
    background: linear-gradient(180deg, var(--color-blue-dark) 10%, var(--color-blue) 90%);
    height: 100vh;
    color: var(--color-white);
    overflow: hidden;

    &--heading {
      font-size: clamp(3rem, 10vw, 5.625rem);
      max-width: 15ch;
      line-height: clamp(60px, 10vw, 101.5px);
      padding: 1rem;
      z-index: 6;
      color: var(--color-white);
    }

    &--inner {
      text-align: center;
      display: flex;
      flex-direction: column;
      justify-content: center;
      align-items: center;
      height: 100%;
      width: 100%;


      & p {
        max-width: 50ch;
        z-index: 6;
      }
    }
  }

  .planet--large {
    position: absolute;
    top: 90%;
    left: 0;
    width: clamp(12rem, 25vw, 20rem);
    z-index: 3;
    animation: bounceLarge 5s infinite ease-in-out;
    
    @media only screen and (max-width: 600px) {
      top: 100%;
      transform: translate(-0%, -100%);
    }
  }
  
  @keyframes bounceLarge {
    0%, 100% {
      transform: translate(-0%, -90%);
    }
    50% {
      transform: translate(-0%, -92%);
    }
  }

  .planet--small {
    position: absolute;
    top: 30%;
    right: -1%;
    width: clamp(7rem, 10vw, 20rem);
    z-index: 3;
    animation: bounceSmall 3s infinite ease-in-out;

    @media only screen and (max-width: 600px) {
      top: 15%;
      transform: translate(-0%, -15%);
    }
  }

  @keyframes bounceSmall {
    0%, 100% {
      transform: translate(-0%, -30%);
    }
    50% {
      transform: translate(-0%, -32%);
    }
  }

  .planet--brand {
    position: absolute;
    left: 50%;
    top:-3rem;
    display: none;
    transform: rotate(-6deg);

  }

  .star {
    position: absolute;
    animation: grow 3s infinite ease;
    z-index: 2;
  }
  
  @keyframes grow {
    0%, 100% {
      // We have to specify the same rotation to create a "random" effect twice 
      // because animating will override the styles already set.
      // So we just set them at all points of the animation
      transform: scale(1) rotate(var(--star-rotate-deg));
    }
    50% {
      transform: scale(1.3) rotate(var(--star-rotate-deg));
    }
  }

  .move-up {
    animation: moveUp 3s infinite;
  }


</style>