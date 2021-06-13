[Elmish]: #/docs/elmish
# Stores

> To check how to work with elmish check out [Elmish]

Sutil uses the concept of stores (observable objects that *store* state) to manage state in a web application (or part of it) and they are really convenient when it comes to reflect state updates in the UI, since they don't require a lot of boilerplate for smaller components it might be the preferred way to handle state. Let's see with an example


In this example we'll try to open and close an ***Alert*** which looks like this
<sl-alert open closable> <sl-icon slot="icon" name="info-circle"></sl-icon> This is a standard alert. You can customize its content and even the icon.
</sl-alert>

If you click on the close button it effectively closes the alert, but how can we open that again programatically, the F# code would look like this

```fsharp
open Sutil
open Sutil.Store
open Sutil.Attr
open Sutil.DOM
// you can open type Shoelace well!
open type Sutil.Shoelace

let view() = 
    Html.article [
        SlAlert [
            Attr.custom("open", "true")
            Attr.custom("closable", "true")
            SlIcon [ Attr.slot "icon"; Attr.name "info-circle" ]
            text "This is a standard alert. You can customize its content and even the icon."
        ]
    ]
```

We'll now add a checkbox that will toggle the alert open and closed also, it will check/uncheck itself itself everytime the alert opens or closes like this

<sl-checkbox id="alert-toggler" checked>Toggle Alert</sl-checkbox>

<sl-alert id="openme" open closable> <sl-icon slot="icon" name="info-circle"></sl-icon> This is a standard alert. You can customize its content and even the icon.
</sl-alert>

To accomplish that we'll use a single store for both elements

```fsharp
let view() =
    // create an store with a bool value
    let isOpen = Store.make true
    Html.article [
        // don't forget to disposte the store
        // when the component gets disposed
        disposeOnUnmount [ isOpen ]
        SlCheckbox [
            Bind.attr("checked", isOpen)
            text "Toggle Alert"
        ]
        SlAlert [
            Bind.attr("open", isOpen)
            Attr.custom("closable", "true")
            SlIcon [ Attr.slot "icon"; Attr.name "info-circle" ]
            text "This is a standard alert. You can customize its content and even the icon."
        ]
    ]
```

In this case that's all we need to do, the reason is that both `SlAlert` and `SlCheckbox` reflect their `open` and `checked` attribute accordingly and the binding engine tracks those changes and updates the store.


```fsharp
open Sutil
open Sutil.Store
open Sutil.Attr
open Sutil.DOM
// you can open type Shoelace well!
open type Sutil.Shoelace

let view() =
    // create an store with a bool value
    let isOpen = Store.make true
    Html.article [
        // don't forget to disposte the store
        // when the component gets disposed
        disposeOnUnmount [ isOpen ]
        SlCheckbox [
            Bind.attr("checked", isOpen)
            text "Toggle Alert"
        ]
        SlAlert [
            Bind.attr("open", isOpen)
            Attr.custom("closable", "true")
            SlIcon [ Attr.slot "icon"; Attr.name "info-circle" ]
            text "This is a standard alert. You can customize its content and even the icon."
        ]
    ]
```


#### Quality of life improvement helpers

Sutil.Shoelace adds a standard way to bind multiple attributes at once in a single store because it might get cumbersome when you have multiple stores tracking multiple values all over the place.


```fsharp
open Sutil
open Sutil.DOM
open Sutil.Attr
open Sutil.Shoelace
open type Shoelace.Shoelace
open Sutil.Shoelace.Alert
open Sutil.Shoelace.Checkbox

let view (page: string) =
  let alertAttrs =
    Store.make (
      // create an attribute record and set the initial values
      SlAlertAttributes.create ()
      |> SlAlertAttributes.withOpen false
      |> SlAlertAttributes.withType "primary"
      |> SlAlertAttributes.withClosable false
    )

  let onChkChange (e: Browser.Types.Event) =
    // the SlCheckbox is a binding of the native Web Component
    let target = (e.target :?> SlCheckbox)
    Store.modify (SlAlertAttributes.withClosable target.``checked``) alertAttrs

  let onBtnClick _ =
    // try to open the alert if it has been closed
    Store.modify (SlAlertAttributes.withOpen true) alertAttrs

  Html.article [
    // don't forget to disposte the store
    // when the component gets disposed
    disposeOnUnmount [ alertAttrs ]
    SlCheckbox [
      text "Closable"
      // react to multiple events that might be affecting your components
      on "sl-change" onChkChange []
    ]
    SlButton [
      text "Open Alert"
      // react to multiple events that might be affecting your components
      onClick onBtnClick []
    ]
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
  ]
```

This is an example that might show when you want to do something like this also, keep in mind that since you're using `Stores` (`Observables`) you should be able to filter, map, different values to different stores with some of the Store's methods. The sky is the limit


<script type="text/javascript">
    var chbox = document.querySelector("#alert-toggler")
    var slalert = document.querySelector("#openme")
    chbox.addEventListener("sl-change", e => slalert.open = e.target.checked)
    slalert.addEventListener("sl-hide", e => chbox.checked = false)
</script>