[Javascript Modules]: https://developer.mozilla.org/en-US/docs/Web/JavaScript/Guide/Modules
[femto]: https://github.com/Zaid-Ajaj/Femto

# Getting Started

```sh
dotnet add package Sutil.Shoelace --version <VERSION>
pnpm install @shoelace-style/shoelace@<VERSION> # or npm install @shoelace-style/shoelace@<VERSION>
```
If you are using [femto] then you just need to do

- `femto install Sutil.Shoelace --version <VERSION>`

# After Install

> Please feel free to also check shoelace's [documentation](https://shoelace.style/getting-started/installation).


Once you're done installing Sutil.Shoelace, you should be able to do the following:

```fsharp
open Sutil.Shoelace

Shoelace.SlButton [
    type' "primary"
    text "Hello, there!"
]
```

but that doesn't mean that you will see yout components right away, Shoelace uses [Javascript Modules] to load its components.

To do that we need to load the components in one of the following ways in your `Main.fs` or entry point file of your Sutil application

```fsharp
// Main.fs or App.fs

importSideEffects "@shoelace-style/shoelace/dist/themes/base.css"
// In case you want to support a dark theme check the guide at
// https://shoelace.style/getting-started/themes?id=dark-mode
// importSideEffects "@shoelace-style/shoelace/dist/themes/dark.css"

importSideEffects "@shoelace-style/shoelace.js"
```
This will load all the shoelace components to the browserand then you should be able to see the following

<sl-button type='primary'>Hello, there!</sl-button>

For development purposes this should be enough. If you want to optimize for production I'd encourage you to keep reading the next section
## Cherry-pick components

> Please feel free to also check shoelace's [documentation](https://shoelace.style/getting-started/installation?id=bundling)


To optimize your bundle sizes and prevent unused code ending up in your user's storage, then cherry-picking is the ideal option since you will only load and bundle the components you're using.

```fsharp

importSideEffects "@shoelace-style/shoelace/dist/components/button/button.js"

importSideEffects "@shoelace-style/shoelace/dist/components/icon/icon.js"
```

then if you try the following:
```fsharp
Shoelace.SlButton [
    Shoelace.SlIcon [
        Attr.slot "prefix"
        Attr.name "info-circle"
    ]
]
```
 it should look like this

<sl-button type="info">
</sl-button>

If you wonder why the icon is not showing it  means that you're not serving the icons statically

you can fix that by selecting where are you going to mount the icon assets with your bundler

```fsharp
[<ImportMember("@shoelace-style/shoelace/dist/utilities/base-path.js")>]
let setBasePath (path: string) : unit = jsNative

setBasePath "shoelace"
```


### Snowpack
In the case of snowpack its quite simple

```javascript
/** @type {import("snowpack").SnowpackUserConfig } */
module.exports = {

    mount: {
        public: { url: '/', static: true },
        src: { url: '/dist' },
        // tell snowpack to mount your assets in "shoelace/assets"
        'node_modules/@shoelace-style/shoelace/dist/assets': { url: '/shoelace/assets', static: true }
        /* ... the rest of your config */
    },
    /* ... the rest of your config */
};
```

### Webpack
In the case of webpack we can use the copy plugin
```javascript
module.exports = {
  /* ... the rest of your config ... */
  plugins: [
    new CopyPlugin({
      patterns: [
        // Copy Shoelace assets to dist/shoelace
        {
          from: path.resolve(__dirname, 'node_modules/@shoelace-style/shoelace/dist/assets'),
          to: path.resolve(__dirname, 'dist/shoelace/assets')
        }
      ]
    })
    /* ... the rest of your config ... */
  ]
};
```

Once we've done that either with webpack or snowpack our button will show like this

<sl-button type="info">
    <sl-icon slot="prefix" name="info-circle">Info</sl-icon>
</sl-button>

Now you're ready to start doing some Sutil.Shoelace!