<svelte:options tag="uw-section"/>

<script>
    // the type of section changes the colors!
    export let type;
</script>

<section class="bg-{type}">
    <div class="section--inner">
        <slot/>
    </div>

</section>

<style lang="scss">
  .section--inner{
    margin-left: 102px;
    margin-right: 102px;
    padding-top: rem(102px);
    padding-bottom: rem(102px);
    
    @media (max-width: 600px){
      margin-left: 30px;
      margin-right: 30px;
    }
  }
</style>