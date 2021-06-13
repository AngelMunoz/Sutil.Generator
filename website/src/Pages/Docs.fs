[<RequireQualifiedAccess>]
module Pages.Docs

open Sutil
open Sutil.DOM
open Sutil.Shoelace
open Sutil.Styling
open Sutil.Attr

let view (page: string) =
  Html.article [
    class' "doc-page"
    Shoelace.SlInclude [
      Attr.src $"/dist/docs/{page}.html"
      Attr.custom ("allow-scripts", "true")
      Html.p [
        text
          "We were not able to find that page, are you sure the url address is correct?"
      ]
    ]
  ]
