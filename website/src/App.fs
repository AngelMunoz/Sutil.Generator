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
    "/docs/:page"
    (fun (mtc: Match<{| page: string option |}, _> option) ->
      match mtc with
      | Some mtc ->
          match mtc.data with
          | Some data ->
              data.page
              |> Option.defaultValue "getting-started"
              |> Docs
              |> navigateTo
          | None -> navigateTo (Docs("getting-started"))
      | None -> navigateTo (Docs("getting-started")))
  |> ignore

  Router.on "/about" (fun _ -> navigateTo About)
  |> ignore

  Router
    .notFound(fun _ -> navigateTo NotFound)
    .resolve()

  let page =
    let location = getCurrentLocation ()

    match location with
    | [||] -> Page.Home
    | [| "about" |] -> Page.About
    | [| "docs"; name |] -> Page.Docs name
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
          text "Documentation"
          Shoelace.SlIcon [
            Attr.name "book"
            Attr.slot "prefix"
          ]
          onClick (fun _ -> Router.navigate "/docs/getting-started" None) []
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
           | Home -> Pages.Docs.view "index"
           | About -> Html.article [ text "About" ]
           | Docs name -> Pages.Docs.view (name)
           | NotFound -> Html.article [ text "NotFound" ]
    ]

  ]
