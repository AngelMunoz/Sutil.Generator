module App

open Browser.Dom
open Browser.Types
open Fable.Core
open Fable.Core.DynamicExtensions
open Sutil
open Sutil.DOM
open Sutil.Attr
open Sutil.Styling
open Sutil.Shoelace
open Sutil.Fast
open Types
open Router
open Components
open Sutil.Fast.FastDesignSystemProvider

type State = { page: Page; navOpen: bool }

type Msg =
  | NavigateTo of Page
  | ToggleNav

[<ImportMember("./Theme.js")>]
let registerThemeEventListener (onThemeChange: bool -> unit) = jsNative

[<ImportMember("./Theme.js")>]
let getPalette (color: string) = jsNative

let init () : State * Cmd<Msg> =
  { page = Home; navOpen = false }, Cmd.none

let update (msg: Msg) (state: State) : State * Cmd<Msg> =
  match msg with
  | NavigateTo page -> { state with page = page }, Cmd.none
  | ToggleNav ->
      { state with
          navOpen = not state.navOpen },
      Cmd.none

let navigateTo (dispatch: Dispatch<Msg>) (page: Page) =
  NavigateTo page |> dispatch

let view () =
  let state, dispatch = Store.makeElmish init update ignore ()

  let theme =
    Store.make
      {| luminance = 1.
         backgroundColor = "#E1E1E1" |}

  let navigateTo = navigateTo dispatch

  let changeTheme isDark =
    if isDark then
      document.body.classList.add ("sl-theme-dark")

      theme
      <~ {| luminance = 0.23
            backgroundColor = "#1E1E1E" |}
    else
      document.body.classList.remove ("sl-theme-dark")

      theme
      <~ {| luminance = 1.
            backgroundColor = "#FFFFFF" |}

  registerThemeEventListener changeTheme

  Router.on "/" (fun _ -> navigateTo Home) |> ignore

  Router.on
    ":library/docs/:page"
    (fun (mtc: Match<DocsUrlData, _> option) ->
      match mtc with
      | Some mtc ->
          match mtc.data with
          | Some { library = library; page = None } ->
              let page = "getting-started"
              let docs = Docs(library, page)
              navigateTo docs
          | Some { library = library; page = page } ->
              let page =
                page |> Option.defaultValue "getting-started"

              let docs = Docs(library, page)
              navigateTo docs
          | None -> navigateTo Home
      | None -> navigateTo Home)
  |> ignore

  Router
    .notFound(fun _ -> navigateTo NotFound)
    .resolve()

  let (|Shoelace|Fast|) =
    function
    | "fast" -> Fast
    | _ -> Shoelace

  let page =
    let location = getCurrentLocation ()

    match location with
    | [||] -> Page.Home
    | [| "fast"; "docs"; name |] -> Page.Docs("fast", name)
    | [| "shoelace"; "docs"; name |] -> Page.Docs("shoelace", name)
    | _ -> Page.NotFound

  navigateTo page


  let background =
    theme .> (fun theme -> theme.backgroundColor)

  let luminance = theme .> (fun theme -> theme.luminance)

  let setColorPallete (e: Event) =
    let el =
      // We'll cast this heare for clarity although, it is not necessary
      e.target :?> FastDesignSystemProvider
    // accentPallete is not an attribute hence why we'll assign it dynamically
    el.["accentPalette"] <- getPalette "#0ea5e9"

  Html.app [
    disposeOnUnmount [ state ]
    Fast.FastDesignSystemProvider [
      class' "main-provider"
      onMount setColorPallete []
      Attr.custom ("use-defaults", "true")
      Attr.custom ("density", "1")
      Attr.custom ("corner-radius", "5")
      // workaround we usually would bind to the attribute not the property
      Bind.attr ("backgroundColor", background)
      Bind.attr ("baseLayerLuminance", luminance)
      Html.nav [
        class' "app-nav"
        Html.section [
          Shoelace.SlButton [
            type' "text"
            text "Sutil.Shoelace"
            onClick (fun _ -> Router.navigate "/" None) []
          ]
          Shoelace.SlButton [
            type' "text"
            text "Shoelace"
            Shoelace.SlIcon [
              Attr.name "book"
              Attr.slot "prefix"
            ]
            onClick (fun _ -> Router.navigate "shoelace/docs/index" None) []
          ]
          Shoelace.SlButton [
            type' "text"
            text "Fast"
            Shoelace.SlIcon [
              Attr.name "book"
              Attr.slot "prefix"
            ]
            onClick (fun _ -> Router.navigate "fast/docs/index" None) []
          ]
        ]
        Html.section [
          class' "show-on-mobile"
          Shoelace.SlButton [
            type' "text"
            text "Menu"
            Shoelace.SlIcon [
              Attr.name "list"
              Attr.slot "prefix"
            ]
            onClick (fun _ -> dispatch ToggleNav) []
          ]
        ]
      ]
      Html.main [
        Desktop.Sidenav
        Mobile.Sidenav [
          Bind.attr ("open", (state .> fun state -> state.navOpen))
          on "sl-hide" (fun _ -> dispatch ToggleNav) []
        ]

        bindFragment state
        <| fun state ->
             match state.page with
             | Home -> Pages.Home.view ()
             | Docs (library, page) -> Pages.Docs.view library page
             | Library library ->
                 match library with
                 | Shoelace -> Pages.Shoelace.view ()
                 | Fast -> Pages.Fast.view ()
             | NotFound -> Html.article [ text "NotFound" ]
      ]
    ]

  ]
