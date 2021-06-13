[Stores]: #/docs/stores

# Elmish

> To check how to work without elmish check out [Stores]

Working with elmish is fairly simple Sutil provides a few Store helpers that allow you to use elmish based components in a sutil'ish way

```fsharp
let state, dispatch = Store.makeElmishSimple init update ignore ()
// and
let state, dispatch = Store.makeElmish init update ignore ()
```
Differences:
- `state` is now an `IStore<State>` rather than just `State`
- we can also dispose things when this elmish component is disposed (pass a function which disposes things instead of ignore)
- The initial params are the last parameter

Let's say for example you have the following elmish component:

```fsharp
open Sutil
open Sutil.Store
open Sutil.Attr
open Sutil.DOM
// you can open Shoelace statically as well!
open Sutil.Shoelace

type State = { isOpen: bool }

type Msg = 
    | SetIsOpen of bool

let init() =
    { isOpen = false }

let update (msg: Msg) (state: State) = 
    match msg with
    | SetIsOpen isOpen -> { state with isOpen = isOpen }

let view() =
    let state, dispatch = Store.makeElmishSimple init update ignore ()
    Html.article [
        // don't forget to disposte the store
        // when the component gets disposed
        disposeOnUnmount [ state ]

        Shoelace.SlButton [ text "Open Externally" ]
        Shoelace.SlMenu [
            Shoelace.SlButton [ Attr.slot "trigger"; Attr.custom("caret", "true"); text "Edit" ]
            Shoelace.SlMenu [
                Shoelace.SlMenuItem [ text "Cut" ]
                Shoelace.SlMenuItem [ text "Copy" ]
                Shoelace.SlMenuItem [ text "Paste" ]
                Shoelace.SlMenuDivider []
                Shoelace.SlMenuItem [ text "Find" ]
                Shoelace.SlMenuItem [ text "Replace" ]
            ]
        ]
    ]
```
If you click on the menu, it will work by itself, but if you wanted to do that from a different element that is not the menu, you'd like to trace such event with the elmish loop

<section>
    <sl-button>Open Externally</sl-button>
    <sl-dropdown>
        <sl-button slot="trigger" caret>Edit</sl-button>
        <sl-menu>
            <sl-menu-item>Cut</sl-menu-item>
            <sl-menu-item>Copy</sl-menu-item>
            <sl-menu-item>Paste</sl-menu-item>
            <sl-menu-divider></sl-menu-divider>
            <sl-menu-item>Find</sl-menu-item>
            <sl-menu-item>Replace</sl-menu-item>
        </sl-menu>
    </sl-dropdown>
</section>

the normal thing here would be to dispatch an event

```fsharp
Shoelace.SlButton [ text "Open Externally"; onClick (fun _ -> dispatch (SetIsOpen true)) [] ]
```
this would trigger our elmish update, let's check how to bind that to our components

```fsharp
let state, dispatch = Store.makeElmishSimple init update ignore ()
// We'll create an observable that reads only the isOpen property
let isOpenHelper = Store.map (fun state -> state.isOpen) state

Html.article [
    Shoelace.SlButton [ text "Open Externally"; onClick (fun _ -> dispatch (SetIsOpen true)) [] ]
    Shoelace.SlMenu [
        // Bind.attr takes an observable
        // each time isOpen changes it will also flect these changes on the element
        Bind.attr("open", isOpenHelper)
        Shoelace.SlButton [ Attr.slot "trigger"; Attr.custom("caret", "true"); text "Edit" ]
        // ... omit the rest for brebity ...
    ]
]
```

<section>
    <sl-button onclick="document.querySelector('#openme').setAttribute('open', '')">Open Externally</sl-button>
    <sl-dropdown id="openme">
        <sl-button slot="trigger" caret>Edit</sl-button>
        <sl-menu>
            <sl-menu-item>Cut</sl-menu-item>
            <sl-menu-item>Copy</sl-menu-item>
            <sl-menu-item>Paste</sl-menu-item>
            <sl-menu-divider></sl-menu-divider>
            <sl-menu-item>Find</sl-menu-item>
            <sl-menu-item>Replace</sl-menu-item>
        </sl-menu>
    </sl-dropdown>
</section>

You can use this technique to bind any kind of value you want, be it either checked, open, closable, label you name it, binding stores and observables fits pretty well the elmish way of doing things

## Complete sample

```fsharp
open Sutil
open Sutil.Store
open Sutil.Attr
open Sutil.DOM
// this time we'll do an open type
open type Sutil.Shoelace

type State = { isOpen: bool }

type Msg = 
    | SetIsOpen of bool

let init() =
    { isOpen = false }

let update (msg: Msg) (state: State) = 
    match msg with
    | SetIsOpen isOpen -> { state with isOpen = isOpen }

let view() =
    let state, dispatch = Store.makeElmishSimple init update ignore ()
    // We'll create an observable that reads only the isOpen property
    let isOpen = Store.map (fun state -> state.isOpen) state

    Html.article [
        // don't forget to disposte the store
        // when the component gets disposed
        disposeOnUnmount [ state ]

        SlButton [
            text "Open Externally"
            // trigger the elmish update
            onClick (fun _ -> dispatch (SetIsOpen true)) []
        ]
        SlMenu [
            // bind the observable
            Bind.attr("open", isOpen)
            SlButton [ Attr.slot "trigger"; Attr.custom("caret", "true"); text "Edit" ]
            SlMenu [
                SlMenuItem [ text "Cut" ]
                SlMenuItem [ text "Copy" ]
                SlMenuItem [ text "Paste" ]
                SlMenuDivider []
                SlMenuItem [ text "Find" ]
                SlMenuItem [ text "Replace" ]
            ]
        ]
    ]
```