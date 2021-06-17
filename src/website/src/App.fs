module App

open Sutil
open Sutil.DOM
open Sutil.Attr
open Sutil.Styling
open Sutil.Shoelace
open Types
open Router
open Components

type State = { page: Page; navOpen: bool }

type Msg =
  | NavigateTo of Page
  | ToggleNav


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

  let navigateTo = navigateTo dispatch

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

  Html.app [
    disposeOnUnmount [ state ]
    class' "app-content"
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
          text "Documentation"
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
