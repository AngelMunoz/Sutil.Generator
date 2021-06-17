/** @type {import("snowpack").SnowpackUserConfig } */
module.exports = {

    mount: {
        public: { url: '/', static: true },
        src: { url: '/dist' },
        'node_modules/@shoelace-style/shoelace/dist/assets': { url: '/shoelace/assets', static: true },
        '../Sutil.Shoelace': { url: '/Sutil.Shoelace', static: true }
    },
    plugins: [
        '@snowpack/plugin-dotenv',
        './markdown.pl.js'
    ],
    routes: [],
    optimize: {
        /* Example: Bundle your final build: */
        bundle: true,
        splitting: true,
        treeshake: true,
        manifest: true,
        target: 'es2017',
        minify: true
    },
    packageOptions: {
        /* ... */
        polyfillNode: true
    },
    devOptions: {
        /* ... */
    },
    buildOptions: {
        /* ... */
        clean: true,
        out: "dist",
        htmlFragments: true
    },
    exclude: [
        "**/*.{fs,fsproj}",
        "**/bin/**",
        "**/obj/**"
    ],
    /* ... */
};