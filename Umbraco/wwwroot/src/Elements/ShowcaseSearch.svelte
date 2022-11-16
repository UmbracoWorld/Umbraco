<svelte:options tag="uw-showcases-search"/>
<script>
    import {onMount, beforeUpdate} from "svelte";

    const instantsearch = window.instantsearch;
    const algoliasearch = window.algoliasearch;

    let searchBox;
    let hits;
    let search;
    let pagination;
    let rangeSlider;
    let searchClient;
    let majorRefinementList;
    let stats;
    let featureRefinementList;

    onMount(() => {

        searchClient = algoliasearch('C7Q4UJ2CDE', '3ee6c2ed9eadf375561f5bee78948bc6');
        search = instantsearch({
            indexName: 'dev_showcases',
            searchClient,
        });

        search.addWidgets([
            instantsearch.widgets.searchBox({
                container: searchBox,
                showSubmit: false,
                placeholder: "Type to begin searching...",

            }),

            // instantsearch.widgets.numericMenu({
            //     container: rangeSlider,
            //     attribute: 'majorVersion',
            //     items: [
            //         {label: 'All'},
            //         {label: 'Greater than 9', start: 9, end: 20},
            //         {label: 'Less than 9', start: 0, end: 8},
            //     ],
            // }),

            // customHits({
            //     container: hits,
            // }),

            instantsearch.widgets.refinementList({
                container: majorRefinementList,
                attribute: 'majorVersion',
                sortBy(a, b) {
                    return a - b;
                },
                cssClasses: {
                    item: ['d-inline-block me-2']
                },
                templates: {
                    item(item, {html}) {
                        const {url, label, count, isRefined} = item;

                        return html`
                            <a class="btn btn--white ${isRefined ? 'btn--primary' : ''}" href="${url}">
                                <span>${label}</span>
                            </a>
                        `;
                    },
                },
            }),

            instantsearch.widgets.refinementList({
                container: featureRefinementList,
                attribute: 'features',
                sortBy(a, b) {
                    return a - b;
                },
                cssClasses: {
                    // item: ['showcases--filters--item']
                },
                templates: {
                    item(item, {html}) {
                        const {url, label, count, isRefined} = item;

                        return html`
                            <a class="btn btn--white ${isRefined ? 'btn--primary' : ''}" href="${url}">
                                <span>${label}</span>
                            </a>
                        `;
                    },
                },
            }),

            instantsearch.widgets.hits({
                container: hits,
                cssClasses: {
                    root: ['container'],
                    list: ['row'],
                    item: ['p-2 col-12 col-md-6'],
                    empty: ['empty']
                },
                templates: {
                    empty(results, { html }) {
                        return html`<div class="showcases--empty">No results found for <q>${results.query}</q></div>`;
                    },
                    item(hit, {html, components}) {
                        return html`
                            <uw-card
                                    title="${instantsearch.highlight({attribute: 'title', hit: hit})}"
                                    body="${instantsearch.highlight({attribute: 'summary', hit: hit})}"
                                    author_key="${hit.authorId}"
                                    date="${hit.dateCreated}"
                                    image_src="${hit.imageSource}">

                            </uw-card>    `;
                    },
                },
            }),

            instantsearch.widgets.pagination({
                container: pagination,
            }),

            instantsearch.widgets.stats({
                container: stats,
            }),
        ])
        ;

        search.start();
    })

</script>

<form bind:this={searchBox} class="text-white pb-5"></form>
<div class="showcases">
    <div class="showcases--left text-white">
        <div class="showcases--left__inner">

            <h3>Filter results</h3>
            <div class="showcases--left__inner--item">
                Major version
                <div bind:this={majorRefinementList}></div>
            </div>

            <div class="showcases--left__inner--item">
                Feature
                <div bind:this={featureRefinementList}></div>
            </div>

            <div class="pt-5" bind:this={stats}></div>
        </div>
    </div>
    <div class="showcases--right">

        <div bind:this={hits}></div>

        <div bind:this={pagination}></div>
    </div>
</div>


<style lang="scss">
  @import "https://cdn.jsdelivr.net/npm/instantsearch.css@7.4.5/themes/reset-min.css";
  @import "././build/bundle.css";
</style>