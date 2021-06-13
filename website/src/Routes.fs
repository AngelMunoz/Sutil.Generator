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

type DocsRoute =
  { name: string
    href: string
    category: Category }


let routes =
  [ { name = "Getting Started"
      href = "#/docs/getting-started"
      category = Guides }
    { name = "Elmish"
      href = "#/docs/elmish"
      category = Guides }
    { name = "Stores"
      href = "#/docs/stores"
      category = Guides } ]
