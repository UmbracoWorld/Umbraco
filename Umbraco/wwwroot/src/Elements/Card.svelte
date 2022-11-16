<svelte:options tag="uw-card"/>
<script>
    import {onMount} from "svelte";

    export let href;
    export let image_src;
    export let title;
    export let body;
    export let profile_src;
    export let profile_image_alt;
    export let author_name;
    export let date;
    export let author_key;
    export let slug;

    let formatDate
    onMount(() => {
        if (!author_name) {
            console.log("no name", author_key);
            fetch("/umbraco/api/AuthorInfo/GetAuthorSummary?key=" + author_key)
                .then((resp) => resp.json())
                .then((data) => {
                    author_name = data.name;
                    slug = data.slug;
                    profile_src = data.profilePictureSource;
                    profile_image_alt = data.profilePicturealt;
                })
        }
        const options = {  year: 'numeric', month: 'short', day: 'numeric' };
        const language = window.navigator.userLanguage || window.navigator.language;
        formatDate = new Date(date).toLocaleString(language, options);
    })


</script>


<div class="card">
    <a href={href} class="underline">
        {#if image_src}
            <div class="card--image">
                <img src={image_src} alt={title}/>
            </div>
        {/if}
        <div class="card--title">
            {#if title}
                <h3>{@html title}</h3>
            {/if}
        </div>
        <div class="card--body">
            {@html body}
        </div>
        <div class="card--meta  text-primary text-uppercase">

            <div class="card--meta__author">
                <img src={profile_src} alt="{author_name} profile picture" width="40" height="40"/>
                {author_name}
            </div>
            <div class="card--meta__date">
                {formatDate}
            </div>
        </div>
    </a>
</div>

<style lang="scss">
  @import "././build/bundle.css";
  .card {
    background: var(--color-white);
    border-radius: 20px;
    padding: 1rem;

    & a {
      color: var(--color-black);
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
      margin-top: 1rem;
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