[<AutoOpen>]
module Components.Sidenav

open Sutil
open Sutil.Attr
open Sutil.Shoelace

open Routes
open Sutil.DOM

let categorized =
  routes |> List.groupBy (fun r -> r.category)


module Desktop =
  let Sidenav =
    Html.custom (
      "menu",
      [ class' "site-menu desktop"
        Html.ul [
          class' "menu-categories"
          for (category, routes) in categorized do
            Html.li [
              class' "menu-categories-category"
              Html.h4 [ text (category.AsString()) ]

              Html.ul [
                for value in routes do
                  Html.li [
                    Shoelace.SlButton [
                      type' "text"
                      text value.name
                      Attr.href value.href
                    ]
                  ]
              ]
            ]
        ] ]
    )



module Mobile =
  let Sidenav (content: NodeFactory seq) =
    Shoelace.SlDrawer [
      yield! content
      class' "site-menu"
      Html.label [
        Attr.slot "label"
        text "Sutil.Shoelace Documentation"
      ]
      Html.ul [
        class' "menu-categories"
        for (category, routes) in categorized do
          Html.li [
            class' "menu-categories-category"
            Html.h4 [ text (category.AsString()) ]

            Html.ul [
              for value in routes do
                Html.li [
                  Shoelace.SlButton [
                    type' "text"
                    text value.name
                    Attr.href value.href
                  ]
                ]
            ]
          ]
      ]
    ]
