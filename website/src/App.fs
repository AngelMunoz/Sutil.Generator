module App

open Sutil
open Sutil.DOM
open Sutil.Attr
open Sutil.Styling
open Sutil.Shoelace
open Types
open Router

type State = { page: Page }

type Msg = NavigateTo of Page

let init () : State * Cmd<Msg> = { page = Home }, Cmd.none

let update (msg: Msg) (state: State) : State * Cmd<Msg> =
  match msg with
  | NavigateTo page -> { state with page = page }, Cmd.none

let navigateTo (dispatch: Dispatch<Msg>) (page: Page) =
  NavigateTo page |> dispatch

let view () =
  let state, dispatch = Store.makeElmish init update ignore ()
  let navigateTo = navigateTo dispatch

  Router.on "/" (fun _ -> navigateTo Home) |> ignore

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
    | _ -> Page.NotFound

  navigateTo page

  Html.app [
    disposeOnUnmount [ state ]
    class' "app-content"
    Html.nav [
      class' "app-nav"
      Shoelace.SlButton [
        type' "text"
        text "Home"
        Shoelace.SlIcon [
          Attr.name "house"
          Attr.slot "prefix"
        ]
        onClick (fun _ -> Router.navigate "/" None) []
      ]
      Shoelace.SlButton [
        type' "text"
        text "About"
        Shoelace.SlIcon [
          Attr.name "info-circle"
          Attr.slot "prefix"
        ]
        onClick (fun _ -> Router.navigate "/about" None) []
      ]
    ]
    Html.main [
      bindFragment state
      <| fun state ->
           match state.page with
           | Home -> Html.article [ text "Home" ]
           | About -> Html.article [ text "About" ]
           | NotFound -> Html.article [ text "NotFound" ]
    ]

  ]
