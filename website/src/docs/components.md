[Shoelace]: https://shoelace.style/

# Sutil.Shoelace Components

> For the full list of components please visit [Shoelace]'s website even though the components have been annotated with doc coments so VSCode, Rider or VS2019 pick them up you can follow the original docs to guide you through the components

We build `Sutil.Shoelace` against a metadata file provided by the npm package, and we follow a few conventions to keep the API consistent and F# friendly

```html
<!-- Shoelace Components have the `sl-` prefix -->
<sl-checkbox checked>I Agree to send some feedback on this lackluster docs website</sl-checkbox>
<sl-alert closable open type="primary">
    <sl-icon slot="icon" name="info-circle">
    This is a standard alert. You can customize its content and even the icon.
</sl-alert>
```
In the case of Sutil.Shoelace we adopt the naming convention used for their React bindings

```fsharp
// prefix with `Sl`
Shoelace.SlAlert
Shoelace.SlButton
Shoelace.SlCheckbox
```

We'll show a more complete example below.

```fsharp
open Sutil.Shoelace
// Optional open the static class so you don't need to type
// Shoelace.Sl<Component> all the time
open type Shoelace.Shoelace
// If you need to access the element's attributes or the 
// binding to the native Web Component open the module
// open Sutil.Shoelace.<Component>
open Sutil.Shoelace.Alert
open Sutil.Shoelace.Checkbox

// the simple call signature is like any other Feliz' flavored DSL
// pass a sequence of attributes/nodes to the component
Shoelace.SlButton [
    // with open type Shoelace.Shoelace
    SlIcon [
        // many components have slots check shoelace's documentation website!
        Attr.slot "prefix"
        name "info-circle"
    ]
    text "My Button"
]

let onChkChange (e: Browser.Types.Event) =
    // the SlCheckbox is a binding of the native Web Component
    // and inside Sutil.Shoelace.Checkbox
    let target = (e.target :?> SlCheckbox)
    printfn $"{target.``checked``}"

SlCheckbox [
    text "I Agree to send some feedback on this lackluster docs website"
    // listen to events on the elements and their children
    // (no need to drill callback props!)
    on "sl-change" onChkChange []
]

// once you open a particular component module you'll have access to it's attributes
let alertAttrs =
    Store.make (
      // create an attribute record and set the initial values
      SlAlertAttributes.create ()
      |> SlAlertAttributes.withOpen false
      |> SlAlertAttributes.withType "primary"
      |> SlAlertAttributes.withClosable false
    )

// the call signature is different, pass an IStore<SlAlertAttributes>
// then your nodes, becareful you might override an existing binding from the
// attributes here if you try to put it again
SlAlert(
    alertAttrs,
    [ SlIcon [
        Attr.slot "icon"
        Attr.name "info-circle"
      ]
      text
        "This is a standard alert. You can customize its content and even the icon." ]
)
```
That's the general idea of how to use them, there is not much more to it.

Feel free to send some feedback in case something is weird or not comprehensible