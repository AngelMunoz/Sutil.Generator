module Router

open Fable.Core

// handler
// hooks
type Route = {| name: string; path: string |}

type Match<'UrlParams, 'QueryParams> =
  {| url: string
     queryString: string
     hashString: string
     Route: Route
     data: 'UrlParams option
     ``params``: 'QueryParams option |}

type RouteHandler<'UrlParams, 'QueryParams> =
  Match<'UrlParams, 'QueryParams> option -> unit

type Router =
  abstract member on :
    string ->
    RouteHandler<'UrlParams, 'QueryParams> ->
    Router

  abstract member notFound : (unit -> unit) -> Router
  abstract member navigate : string -> obj option -> unit
  abstract member navigateByName : string -> 'T option -> unit
  abstract member getCurrentLocation : unit -> Match<_, _>
  abstract member resolve : unit -> unit


[<ImportMember("./Router.js")>]
let Router : Router = jsNative

[<ImportMember("./Router.js")>]
let getCurrentLocation : unit -> string array = jsNative
