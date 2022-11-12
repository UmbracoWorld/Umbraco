<svelte:options tag="uw-navigation-toggle"/>

<script>
    let isActive = false;
    
    // Passed in from the navbar to determine whether to set the css var to white or black
    export let is_dark = false;

    function setActive() {
        isActive = !isActive;
    }

    const background = document.getElementById("nav-links--background");
    Array.from(document.getElementsByClassName("nav-link"))
        .forEach((item, index) => {
            item.onmouseover = () => {
                background.style.backgroundPosition = "0%" + ` ${(index * 20)}%`
            }
        });

</script>

<div class="offcanvas {isActive ? 'is-active' : ''}" id="offcanvas_menu">
    <slot name="links"></slot>
</div>

<button id="burger"
        class="hamburger hamburger--emphatic {isActive ? 'is-active' : ''}"
        on:click={setActive}
        style="--burger-bg-color: {is_dark ? '#000' : '#FFF'} ">
    <div class="hamburger-box">
        <div class="hamburger-inner">
            <span class="hamburger-inner__text d-none d-sm-block">
                {isActive ? 'CLOSE' : 'MENU'}
            </span>
        </div>
    </div>
</button>

<style lang="scss">
  /*!
 * Hamburgers
 * @description Tasty CSS-animated hamburgers
 * @author Jonathan Suh @jonsuh
 * @site https://jonsuh.com/hamburgers
 * @link https://github.com/jonsuh/hamburgers
 */
  :root {
    --burger-bg-color: #000;
  }

  .hamburger {
    padding: 15px 15px;
    display: inline-block;
    cursor: pointer;
    transition-property: opacity, filter;
    transition-duration: 0.15s;
    transition-timing-function: linear;
    font: inherit;
    color: inherit;
    text-transform: none;
    background-color: transparent;
    border: 0;
    margin: 0;
    overflow: visible;
    z-index: 9;
    position: fixed;
    right: 5vw;
    top: 1.5rem;
  }

  .hamburger.is-active {
    right: 1vw;
  }

  .hamburger:hover {
    opacity: 0.7;
  }

  .hamburger.is-active:hover {
    opacity: 0.7;
  }

  .hamburger.is-active .hamburger-inner,
  .hamburger.is-active .hamburger-inner::before,
  .hamburger.is-active .hamburger-inner::after {
    background-color: var(--burger-bg-color-active);
  }

  .hamburger-box {
    width: 40px;
    height: 24px;
    display: inline-block;
    position: relative;
  }


  
  .hamburger-inner {
    display: block;
    top: 50%;
    margin-top: -2px;
    
    &__text {
      color: var(--burger-bg-color);
      position: absolute;
      left: -4rem;
      top: -5px;
      text-transform: uppercase;
      font-weight: 800;
    }
  }

  .hamburger.is-active .hamburger-inner .hamburger-inner__text {
    top: -15px;
    left: -5.5rem;
    font-size: 1.5rem;
    color: var(--burger-bg-color-active) !important;
  }

  .hamburger-inner, .hamburger-inner::before, .hamburger-inner::after {
    width: 40px;
    height: 5px;
    background-color: var(--burger-bg-color);
    border-radius: 30px;
    position: absolute;
    transition-property: transform;
    transition-duration: 0.3s;
    transition-timing-function: ease;
  }

  .hamburger-inner::before, .hamburger-inner::after {
    content: "";
    display: block;
  }

  .hamburger-inner::before {
    /*top: -10px;*/
  }

  .hamburger-inner::after {
    bottom: -10px;
  }


  /*
     * Emphatic
     */
  //.hamburger--emphatic {
  //  overflow: hidden;
  //}

  .hamburger--emphatic .hamburger-inner {
    transition: background-color 0.125s 0.175s ease-in;
  }

  .hamburger--emphatic .hamburger-inner::before {
    left: 0;
    transition: transform 0.125s cubic-bezier(0.6, 0.04, 0.98, 0.335), top 0.05s 0.125s linear, left 0.125s 0.175s ease-in;
  }

  .hamburger--emphatic .hamburger-inner::after {
    top: 10px;
    right: 0;
    transition: transform 0.125s cubic-bezier(0.6, 0.04, 0.98, 0.335), top 0.05s 0.125s linear, right 0.125s 0.175s ease-in;
  }

  .hamburger--emphatic.is-active .hamburger-inner {
    transition-delay: 0s;
    transition-timing-function: ease-out;
    background-color: transparent !important;
  }

  .hamburger--emphatic.is-active .hamburger-inner::before {
    left: -80px;
    top: -80px;
    transform: translate3d(80px, 80px, 0) rotate(45deg);
    transition: left 0.125s ease-out, top 0.05s 0.125s linear, transform 0.125s 0.175s cubic-bezier(0.075, 0.82, 0.165, 1);
  }

  .hamburger--emphatic.is-active .hamburger-inner::after {
    right: -80px;
    top: -80px;
    transform: translate3d(-80px, 80px, 0) rotate(-45deg);
    transition: right 0.125s ease-out, top 0.05s 0.125s linear, transform 0.125s 0.175s cubic-bezier(0.075, 0.82, 0.165, 1);
  }

  //Offcanvas menu
  /* The side navigation menu */
  .offcanvas {
    height: 100%;
    width: 0;
    position: fixed;
    z-index: 7;
    top: 0;
    right: -500px;
    background-color: var(--color-blue-dark);
    overflow-x: hidden;
    padding-top: 120px;
    transition: 0.5s;
    opacity: 0;

    & a {
      padding: 8px 8px 8px 32px;
      text-decoration: none;
      font-size: 25px;
      display: block;
    }

    &.is-active {
      opacity: 1;
      right: 0;
      width: 100vw;
      box-shadow: -5px 0px 20px 5px rgba(0, 0, 0, 0.2);
    }

  }
</style>