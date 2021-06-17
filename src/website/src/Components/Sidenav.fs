[<AutoOpen>]
module Components.Sidenav

open Sutil
open Sutil.Attr
open Sutil.Shoelace

open Routes
open Sutil.DOM

let shoelace =
  routes
  |> List.filter (fun route -> route.library = Shoelace)
  |> List.groupBy (fun r -> r.category)

let fast =
  routes
  |> List.filter (fun route -> route.library = Fast)
  |> List.groupBy (fun r -> r.category)


module Desktop =
  let private libraryMenu (library: list<Category * list<DocsRoute>>) =
    Html.ul [
      class' "menu-categories"
      for (category, routes) in library do
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

  let Sidenav =
    Html.aside [
      class' "site-menu desktop"
      Html.ul [
        Html.h4 [
          Attr.slot "summary"
          text "Shoelace"
        ]
        libraryMenu shoelace
      ]
      Html.ul [
        Html.h4 [
          Attr.slot "summary"
          text "FAST"
        ]
        libraryMenu fast
      ]
    ]



module Mobile =
  let private mobileMenu (library: list<Category * list<DocsRoute>>) =
    Html.ul [
      class' "menu-categories"
      for (category, routes) in library do
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

  let Sidenav (content: NodeFactory seq) =
    Shoelace.SlDrawer [
      yield! content
      class' "site-menu"
      Html.label [
        Attr.slot "label"
        text "Select a library Documentation"
      ]
      Html.ul [
        Html.li [
          Html.h3 "Shoelace"
          mobileMenu shoelace
        ]
      ]
      Html.ul [
        Html.li [
          Html.h3 "Fast"
          mobileMenu fast
        ]
      ]
    ]
