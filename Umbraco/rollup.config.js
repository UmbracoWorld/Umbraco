﻿import svelte from 'rollup-plugin-svelte';
import resolve from '@rollup/plugin-node-resolve';
import postcss from 'rollup-plugin-postcss'
import sveltePreprocess from 'svelte-preprocess';

export default {
    // This `main.js` file we wrote
    input: 'wwwroot/src/main.js',
    output: {
        // The destination for our bundled JavaScript
        file: 'wwwroot/build/bundle.js',
        // Our bundle will be an Immediately-Invoked Function Expression
        format: 'iife',
        // The IIFE return value will be assigned into a variable called `app`
        name: 'app',
    },
    plugins: [
        postcss({
            extract: 'bundle.css',
            minimize: true,
            use: ['sass']
        }),
        svelte({
            preprocess: sveltePreprocess({
                scss: {
                    prependData: `@import 'wwwroot/src/styles/main.scss';`
                }
            }),
            // Tell the svelte plugin where our svelte files are located
            include: 'wwwroot/**/*.svelte',
            emitCss: false,
            compilerOptions: {
                customElement: true
            },
        }),

        // Tell any third-party plugins that we're building for the browser
        resolve({browser: true,}),
    ]
};