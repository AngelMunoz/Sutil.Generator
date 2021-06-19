module Routes

type Category =
  | Guides
  | Components
  | Uncategorized

  member this.AsString() =
    match this with
    | Guides -> "Guides"
    | Components -> "Components"
    | Uncategorized -> ""

type Library =
  | Shoelace
  | Fast

type DocsRoute =
  { name: string
    href: string
    library: Library
    category: Category }


let routes =
  [ { name = "Getting Started"
      href = "#/shoelace/docs/getting-started"
      library = Shoelace
      category = Guides }
    { name = "Getting Started"
      href = "#/fast/docs/getting-started"
      library = Fast
      category = Guides }
    { name = "Themes"
      href = "#/fast/docs/themes"
      library = Fast
      category = Guides }
    { name = "How to use components"
      href = "#/shoelace/docs/components"
      library = Shoelace
      category = Guides }
    { name = "Elmish"
      href = "#/shoelace/docs/elmish"
      library = Shoelace
      category = Guides }
    { name = "Stores"
      href = "#/shoelace/docs/stores"
      library = Shoelace
      category = Guides } ]
