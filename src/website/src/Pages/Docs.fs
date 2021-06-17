[<RequireQualifiedAccess>]
module Pages.Docs

open Sutil
open Sutil.DOM
open Sutil.Shoelace
open Sutil.Styling
open Sutil.Attr

let view (library: string) (page: string) =
  Html.article [
    class' "doc-page"
    Shoelace.SlInclude [
      Attr.src $"/dist/docs/{library}/{page}.html"
      Attr.custom ("allow-scripts", "true")
    ]
  ]
