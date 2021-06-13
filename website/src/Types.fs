module Types

type Page =
  | Home
  | About
  | Docs of string
  | NotFound
