module Main

open Sutil
open Sutil.DOM
open Fable.Core
open Fable.Core.JsInterop


importSideEffects "@shoelace-style/shoelace/dist/themes/base.css"
importSideEffects "@shoelace-style/shoelace/dist/themes/dark.css"

importSideEffects "./styles.css"
importSideEffects "@shoelace-style/shoelace/dist/shoelace.js"

[<ImportMember("@shoelace-style/shoelace/dist/utilities/base-path.js")>]
let setBasePath (path: string) : unit = jsNative

setBasePath "shoelace"
// Start the app
App.view () |> mountElement "sutil-app"
