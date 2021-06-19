[Themes]: https://www.fast.design/docs/design-systems/overview

# FAST Themes

> Please also check FAST documentation on [Themes] for more and better information.

Fast includes a system design provider which allows you to write theming in your website from the start.

In the case of `Sutil.Fast` once you [have imported](#/fast/docs/getting-started) the required scripts you can do the following

```fsharp
open Sutil.Fast

Fast.FastDesignSystemProvider [
  // initialize all of the properties
  Attr.custom ("use-defaults", "true")
  Attr.custom ("density", "1")
  Attr.custom ("corner-radius", "5")
  Attr.custom ("background-color", "#000000")
  Attr.custom ("base-layer-luminance", "0.23")

  Fast.FastButton [
    Attr.custom("appearance", "accent")
    text "Hey there!"
  ]
]

```
that will give you a dark style with the default accent

<fast-design-system-provider style="padding: 1em" use-defaults density="1" cornder-radius="5" background-color="#000000" base-layer-luminance="0.23">
<fast-button appearance="accent">Hey there!</fast-button>
</fast-design-system-provider>


# Accent color pallete

> Please also note that FAST uses a lot of javascript for creating custom design systems, so in those cases you are advised to simply add a javascript file for interop (unless you have the time to create bindings for the javascript code in the FAST library)



This part is not obligatory but it might be the easiest way to do it until there's a bindings package for the javascript part of FAST
```js
import { parseColorString, createColorPalette } from '@microsoft/fast-components';

// generate a new color palette for the color you chose
export function getPalette(color) {
    return createColorPalette(parseColorString(color));
}
```


```fsharp
open Browser.Types

open Fable.Core
open Fable.Core.DynamicExtensions

open Sutil.Fast
open Sutil.Fast.FastDesignSystemProvider

[<ImportMember("./Theme.js")>]
let getPalette (color: string) = jsNative

let setColorPallete (e: Event) =
  let el =
    // We'll cast this heare for clarity although, it is not necessary
    e.target :?> FastDesignSystemProvider
  // accentPallete is not an attribute hence why we'll assign it dynamically
  el.["accentPalette"] <- getPalette "#0ea5e9"

let view() = 
  Fast.FastDesignSystemProvider [
    // when the component is mounted set the collor palette
    onMount setColorPallete []
    // initialize all of the properties
    Attr.custom ("use-defaults", "true")
    // set custom values for existing attributes
    Attr.custom ("density", "1")
    Attr.custom ("corner-radius", "5")
    Attr.custom ("background-color", "#000000")
    Attr.custom ("base-layer-luminance", "0.23")

    Fast.FastButton [
      Attr.custom("appearance", "accent")
      text "Hey there!"
    ]
  ]
```
That should look like this:

<fast-design-system-provider style="padding: 1em" id="change-me" use-defaults density="1" cornder-radius="5" background-color="#000000" base-layer-luminance="0.23">
<fast-button appearance="accent">Hey there!</fast-button>
</fast-design-system-provider>

<script type="module">
  import {parseColorString, createColorPalette} from "https://unpkg.com/@microsoft/fast-components"
  const provider = document.querySelector("#change-me")
  provider.accentPalette = createColorPalette(parseColorString("#0ea5e9"));
</script>

# Dark/Light Mode

Supporting Light and Dark modes is not too hard and should be simple to do. for that we just need to listen for the media query change, add a store and register a callback to react to those changes.

Let's add a few things to our JS file

```js
import { parseColorString, createColorPalette } from '@microsoft/fast-components';

const prefersDarkQuery = window.matchMedia('(prefers-color-scheme: dark)');
const isDark = () => prefersDarkQuery.matches;

export function registerThemeEventListener(cb) {
    cb(isDark());
    prefersDarkQuery.addEventListener('change', () => cb(isDark()));
}

export function getPalette(color) {
    return createColorPalette(parseColorString(color));
}
```

```fsharp
open Browser.Types

open Fable.Core
open Fable.Core.DynamicExtensions
open Sutil
open Sutil.Fast
open Sutil.Fast.FastDesignSystemProvider


[<ImportMember("./Theme.js")>]
let registerThemeEventListener (onThemeChange: bool -> unit) = jsNative

[<ImportMember("./Theme.js")>]
let getPalette (color: string) = jsNative

let setColorPallete (e: Event) =
  let el =
    // We'll cast this heare for clarity although, it is not necessary
    e.target :?> FastDesignSystemProvider
  // accentPallete is not an attribute hence why we'll assign it dynamically
  el.["accentPalette"] <- getPalette "#0ea5e9"

let changeTheme isDark =
  // theme <~ is the equivalent to 
  // Store.set {| ...values ... |} theme
  if isDark then
    theme
    <~ {| luminance = 0.23
          backgroundColor = "#1E1E1E" |}
  else
    theme
    <~ {| luminance = 1.
          backgroundColor = "#FFFFFF" |}
// register our callback
registerThemeEventListener changeTheme

let background =
  // theme .> (fun theme -> theme.backgroundColor) is the equivalent to
  // Store.map (fun theme -> theme.backgroundColor) theme
  theme .> (fun theme -> theme.backgroundColor)

let luminance = theme .> (fun theme -> theme.luminance)

let view() =
  let theme =
    Store.make
      // Initial values for your theme
      {| luminance = 1.
         backgroundColor = "#1E1E1E" |}
  Fast.FastDesignSystemProvider [
    // when the component is mounted set the collor palette
    onMount setColorPallete []
    // initialize all of the properties
    Attr.custom ("use-defaults", "true")
    // set custom values for existing attributes
    Attr.custom ("density", "1")
    Attr.custom ("corner-radius", "5")
    // workaround we usually would bind to the attribute not the property
    // e.g Bind.attr ("background-color", background)
    Bind.attr ("backgroundColor", background)
    Bind.attr ("baseLayerLuminance", luminance)

    Fast.FastButton [
      Attr.custom("appearance", "accent")
      text "Hey there!"
    ]
  ]
```

That's precisely how this website is configured right now! for the sample we'll inver the colors just to show that you can use multiple providers and have multiple sections with different themes

<fast-design-system-provider style="padding: 1em" id="change-me-too" use-defaults density="1" cornder-radius="5" background-color="#000000" base-layer-luminance="0.23">
<fast-button appearance="accent">Hey there!</fast-button>
</fast-design-system-provider>

<script type="module">
  import {parseColorString, createColorPalette} from "https://unpkg.com/@microsoft/fast-components"
  const prefersDarkQuery = window.matchMedia('(prefers-color-scheme: dark)');
  const provider = document.querySelector("#change-me-too")
  provider.accentPalette = createColorPalette(parseColorString("#0ea5e9"));
  provider.backgroundColor = "#FFFFFF"
  provider.baseLayerLuminance = 1

  prefersDarkQuery.addEventListener('change', () => {
    if(prefersDarkQuery.matches) {
      provider.backgroundColor = "#FFFFFF"
      provider.baseLayerLuminance = 1
    } else {
      provider.backgroundColor = "#1E1E1E"
      provider.baseLayerLuminance = 0.23
    }
  });
</script>
