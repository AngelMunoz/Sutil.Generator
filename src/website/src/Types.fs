module Types

type Page =
  | Home
  | Library of string
  | Docs of library: string * docSite: string
  | NotFound

type Theme =
  | Light
  | Dark

type DocsUrlData =
  { library: string
    page: string option }
