# Shoelace.Sutil Documentation
Sutil.Shoelace Docs are in the `website` directory at the root of the project.

To be able to update existing documents or add them you'll need to:
1. Create a new `markdown` file at `website/src/docs/` in case it doesn't exist.

    Example: `website/src/docs/sl-button.md`
2. Add/Edit the contents of the document.

    you can use markdown or html including script tags. Example:
    
    ```markdown
    # My Article
    My explanation of how an SlButton works 
    '''fsharp
    Shoelace.SlButton [
        type' "primary"
        onClick (fun _ -> printfn "clicked") []
        text "I'm a button"
    ]
    '''
    then a small sample on how would the result be (which can't be done with F# in the website yet)

    <sl-button id="my-button" type="primary">I'm a button</sl-button>
    <script type="module">
        var btn = document.querySelector("#my-button")
        btn.addEventListener("click", () => console.log('clicked'))
    </script>
    ```

3. Once this is done the file will be available at `#/docs/sl-button`.

4. If you feel the need for it add an entry on the menu at `website/src/Routes.fs`.

    In this example you would add something like this inside the routes list
    ```fsharp
    { name = "SlButton"
      href = "#/docs/sl-button"
      category = Components }
    ```


# Shoelace.Generator project

The generator project does the following

1. Download the `@shoelace-style/shoelace` package from the npm registry
2. Reads `./node_modules/@shoelace-style/shoelace/dist/metadata.json`
3. Deserializes it's contents into the types available in `Types.fs`
4. Writes a complete F# project to `./Sutil.Shoelace` including the `.fsproj` file

Each file that is generated corresponds to a component that exists inside the `metadata.json`

If you want to add functionality to the modules/components then you'll need to write the corresponding string template in `Templates.fs` the file is quite messy right now but any help to improve the clarity in this file is welcome.


## Getting to know the end product
For the next section I'll talk about what is expected to have on the generated output although, for brevity I'll take out doc comments written in the final files and put my own comments instead

## Anatomy of a generated file (with attributes)


```fsharp
// declation of the module and open statements
module Sutil.Shoelace.MenuItem
open Browser.Types
open Sutil
open Sutil.DOM

(* Every Shoelace component has a native HTML element ideally we need
   to provide bindings of these so there is typing information
   when the users access them at event handlers
*)
[<AllowNullLiteral>]
type SlMenuItem =
    inherit HTMLElement
    (* there are certain reserved words from F# that are often used in javascript
       in these cases we use double backticks " ` " to preserve the original names *)
    abstract member ``checked`` : bool with get, set
    abstract member disabled : bool with get, set
    abstract member value : string with get, set
    abstract member blur :  unit
    (* in the cases we know what the parameters are we should provide anonymous types for them
       the reason being anonymous types get translated as plain javascript objects in Fable 
       if the type is not known or problematic then obj should be good enough *)
    abstract member focus :  {| preventScroll: bool option |} option -> unit
    
(* IF the component has documented attributes we'll emit a type and module
   named like this `Sl<COMPONENT_NAME>Attributes`
   this is to provide a few helpers described below *)
type SlMenuItemAttributes = { 
    checked' : bool option
    disabled : bool option
    value : string option
}

[<RequireQualifiedAccess>]
module SlMenuItemAttributes  =
    (* every attributes module will have a created function
       that provides an object with optional values and for some
       of them their default values *)
    let create(): SlMenuItemAttributes = { 
        checked' = Some false
        disabled = Some false
        value = Some ""
    }
    
    (* from there on we'll provide simple functions in the form of 
       `with<ATTRIBUTE_NAME_PASCAL_CASE> (<ATTRIBUTE_NAME>: type) (attrs: attribute type)`
       in the case of reserved names since these are handled in the library code
       we can use ` ' ` to sufix the property name and this will not impact the HTMLELement *)
    
    let withChecked (checked': bool) (attrs: SlMenuItemAttributes) =
        { attrs with checked' = Some checked' }
    
    let withDisabled (disabled: bool) (attrs: SlMenuItemAttributes) =
        { attrs with disabled = Some disabled }
    
    
    let withValue (value: string) (attrs: SlMenuItemAttributes) =
        { attrs with value = Some value }
(* We'll provide a module with the name of the component
   and two methods in the case of components with attributes *)
[<RequireQualifiedAccess>]
module SlMenuItem =
    
    (* stateless is a bad name since it's not stateless
       the idea behind is that we're not providing any kind of binding by default
       and we're just providing a simple function that returns an html element *)
    let stateless (content: NodeFactory seq) = Html.custom("sl-menu-item", content)
    (* The "stateful" function provides a simple mechanism to bind all of the attributes
       to the html element via observables and `Bind.attr(<ATTRIBUTE_NAME>, <OBSERVABLE>)` where "ATTRIBUTE_NAME" is the actual html attribute (checked, disabled, no-fieldset (Note: dash cased instead of cammel case when writing these attributes))
       then we just call the stateless function and yield everything to the Html.custom function from Sutil
       *)
    let stateful (attrs: IStore<SlMenuItemAttributes>) (nodes: NodeFactory seq) =
        
        let checked' = attrs .> (fun attrs -> attrs.checked')
        let disabled = attrs .> (fun attrs -> attrs.disabled)
        let value = attrs .> (fun attrs -> attrs.value)
        stateless
            [ Bind.attr("checked", checked')
              Bind.attr("disabled", disabled)
              Bind.attr("value", value)
              yield! nodes ]
```

## Anatomy of a generated file (without attributes)


```fsharp
// declation of the module and open statements
module Sutil.Shoelace.MenuLabel
open Browser.Types
open Sutil
open Sutil.DOM
(* Every Shoelace component has a native HTML element ideally we need
   to provide bindings of these so there is typing information
   when the users access them at event handlers
*)
[<AllowNullLiteral>]
type SlMenuLabel =
    inherit HTMLElement
    
    
(* We'll provide a module with the name of the component
   In cases like this that the components don't provide documented attributes
   we just emit the stateless function *)
[<RequireQualifiedAccess>]
module SlMenuLabel =
    
    let stateless (content: NodeFactory seq) = Html.custom("sl-menu-label", content)
```