<svelte:options tag="uw-card"/>
<script>
    export let href;
    export let image_src;
    export let title;
    export let body;
    export let profile_src;
    export let author_name;
    export let date;
</script>


<div class="card">
    <a href={href}>
        <div class="card--image">
            <img src={image_src} alt={title} width="500" height="300"/>
        </div>
        <div class="card--title">
            <h3>{title}</h3>
        </div>
        <div class="card--body">
            {body}
        </div>
        <div class="card--meta  text-primary text-uppercase">

            <div class="card--meta__author">
                <img src={profile_src} alt="{author_name} profile picture" width="40" height="40"/>
                {author_name}
            </div>
            <div class="card--meta__date">
                {date}
            </div>
        </div>
    </a>
</div>

<style lang="scss">
  .card {
    background: var(--color-white);
    border-radius: 20px;
    padding: 1rem;
    
    & a{
      text-decoration: none;
    }

    &:hover {
      background: var(--color-blue-light);
      cursor: pointer;
    }

    &--image {
      & img {
        border-radius: 1rem;
        width: 100%;
      }
    }

    &--body {
      font-size: rem(16px);
    }

    &--title {
      color: var(--color-primary);
    }

    &--meta {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-top: 2rem;
      font-weight: 700;
      font-size: rem(16px);

      &__author {
        display: flex;
        align-items: center;
        gap: 1rem;
      }

      & img {
        max-width: 40px;
        border-radius: 100px;
      }
    }

  }
</style>