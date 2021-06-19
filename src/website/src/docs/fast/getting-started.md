[Javascript Modules]: https://developer.mozilla.org/en-US/docs/Web/JavaScript/Guide/Modules
[femto]: https://github.com/Zaid-Ajaj/Femto
[Theming]: #/fast/docs/theming

# Getting Started

```sh
dotnet add package Sutil.Fast --version <VERSION>
pnpm install @microsoft/fast-components@<VERSION> @microsoft/fast-foundation lodash-es # or npm install @microsoft/Fast@<VERSION>
```
If you are using [femto] then you just need to do

- `femto install Sutil.Fast --version <VERSION>`

# After Install

> Please feel free to also check Fast's [documentation](https://www.fast.design/docs/introduction/).


Once you're done installing Sutil.Fast, you should be able to do the following:

```fsharp
open Sutil.Fast

Fast.FastDesignSystemProvider [
  Attr.custom ("use-defaults", "true")
  Attr.style "padding: 1em"
  Fast.FastButton [
      Attr.custom("appearance","primary")
      text "Hello, there!"
  ]
]
```

but that doesn't mean that you will see yout components right away, Fast uses [Javascript Modules] to load its components.

To do that we need to load the components in one of the following ways in your `Main.fs` or entry point file of your Sutil application

```fsharp
// Main.fs or App.fs
importSideEffects "@microsoft/fast-components"
```
This will load all the Fast components to the browser and then you should be able to see the following


<fast-design-system-provider use-defaults style="padding: 1em">
  <fast-button appearance='accent'>Hello, there!</fast-button>
</fast-design-system-provider>

Fast uses a design system provider that allows you to easily theme your application or parts of your application as much as you desire, if you want to learn more about that check

<fast-anchor href="#/fast/docs/themes" appearance="outline">Fast Themes</fast-anchor>

For development purposes this should be enough. If you want to optimize for production I'd encourage you to keep reading the next section
## Cherry-pick components

> Please feel free to also check Fast's [documentation](https://www.fast.design/docs/components/getting-started#from-npm)


To optimize your bundle sizes and prevent unused code ending up in your user's storage, then cherry-picking is the ideal option since you will only load and bundle the components you're using.

```fsharp
open Sutil.Fast.FastDesignSystemProvider
open Sutil.Fast.FastButton
open Sutil.Fast.FastAnchor

let private FASTDesignSystemProvider = importMember<FastDesignSystemProvider>"@microsoft/fast-components"
let private FASTButton = importMember<FastButton>"@microsoft/fast-components"
let private FASTAnchor = importMember<FastAnchor>"@microsoft/fast-components"
```

This will help your bundler to prevent tree-shaking the elements that you're using.
Now you're ready to start doing some Sutil.Fast!