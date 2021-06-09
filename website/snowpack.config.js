/** @type {import("snowpack").SnowpackUserConfig } */
module.exports = {
    mount: {
        public: { url: '/', static: true },
        src: { url: '/dist' },
        'node_modules/@shoelace-style/shoelace/dist/assets': { url: '/shoelace/assets', static: true },
        '../Sutil.Shoelace': { url: '/Sutil.Shoelace', static: true }
    },
    plugins: ['@snowpack/plugin-svelte', '@snowpack/plugin-dotenv'],
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
    },
    devOptions: {
        /* ... */
    },
    buildOptions: {
        /* ... */
        clean: true,
        out: "dist"
    },
    exclude: [
        "**/*.{fs,fsproj}",
        "**/bin/**",
        "**/obj/**"
    ],
    /* ... */
};