module Main

open Sutil
open Sutil.DOM
open Fable.Core
open Fable.Core.JsInterop
open Sutil.Fast.FastDesignSystemProvider
open Sutil.Fast.FastButton
open Sutil.Fast.FastAnchor

// CSS Imports

importSideEffects "@shoelace-style/shoelace/dist/themes/base.css"
importSideEffects "@shoelace-style/shoelace/dist/themes/dark.css"
importSideEffects "highlight.js/styles/nord.css"
importSideEffects "firacode/distr/fira_code.css"
importSideEffects "./styles.css"

// JS Imports

importSideEffects "@shoelace-style/shoelace/dist/components/alert/alert.js"
importSideEffects "@shoelace-style/shoelace/dist/components/button/button.js"

importSideEffects
  "@shoelace-style/shoelace/dist/components/checkbox/checkbox.js"

importSideEffects "@shoelace-style/shoelace/dist/components/icon/icon.js"
importSideEffects "@shoelace-style/shoelace/dist/components/menu/menu.js"

importSideEffects
  "@shoelace-style/shoelace/dist/components/menu-divider/menu-divider.js"

importSideEffects
  "@shoelace-style/shoelace/dist/components/menu-item/menu-item.js"

importSideEffects "@shoelace-style/shoelace/dist/components/drawer/drawer.js"
importSideEffects "@shoelace-style/shoelace/dist/components/include/include.js"

importSideEffects
  "@shoelace-style/shoelace/dist/components/dropdown/dropdown.js"

importSideEffects "./Theme.js"

let private FASTDesignSystemProvider =
  importMember<FastDesignSystemProvider> "@microsoft/fast-components"

let private FASTButton =
  importMember<FastButton> "@microsoft/fast-components"

let private FASTAnchor =
  importMember<FastAnchor> "@microsoft/fast-components"



[<ImportMember("@shoelace-style/shoelace/dist/utilities/base-path.js")>]
let setBasePath (path: string) : unit = jsNative

setBasePath "shoelace"
// Start the app
App.view () |> mountElement "sutil-app"
