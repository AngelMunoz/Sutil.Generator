[<RequireQualifiedAccess>]
module Pages.Home

open Sutil
open Sutil.Shoelace
open Sutil.Fast
open Sutil.Styling
open Sutil.DOM
open Sutil.Attr

open type Feliz.length

let view () =
  Html.article [
    Html.section [
      Shoelace.SlInclude [
        Attr.src "/dist/docs/home.html"
      ]
    ]
    Html.section [
      class' "row"
      Fast.FastAnchor [
        Attr.href "https://fast.design"
        Attr.target "_blank"
        Attr.custom ("appearance", "outline")
        text "Get to know more about FAST"
      ]
      Shoelace.SlButton [
        Attr.target "_blank"
        Attr.href "https://shoelace.style"
        text "Get to know more about Shoelace"
      ]
    ]
    Html.section [
      Shoelace.SlInclude [
        Attr.src "/dist/docs/home-2.html"
      ]
    ]
  ]
  |> withStyle [
       rule
         "article"
         [ Css.marginLeft Feliz.length.auto
           Css.marginRight Feliz.length.auto
           Css.marginTop (Feliz.length.em 2)
           Css.padding (Feliz.length.em 2)
           Css.paddingTop 0 ]
       rule
         "section.row"
         [ Css.marginTop (em 2)
           Css.marginBottom (em 2)
           Css.displayFlex
           Css.custom ("justify-content", "space-evenly")
           Css.alignItemsCenter ]
     ]
