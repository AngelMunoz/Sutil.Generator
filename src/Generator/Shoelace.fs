namespace Sutil.Generator.Shoelace
open Sutil.Generator.Types

(*
    DO NOT USE FANTOMAS OR ANOTHER FORMATTER 
    THAT CHANGES THE MULTI-LINE STRING STRUCTURE ON THIS FILE
    IT WILL BREAK THE DOC COMMENTS GENERATION
*)

[<RequireQualifiedAccess>]
module Templates =

    open System.Web


    let getPropName (name: string) =
        match name with
        | "type" -> "``type``"
        | "open" -> "``open``"
        | "inline" -> "``inline``"
        | "checked" -> "``checked``"
        | rest -> rest

    let getAttrName (name: string) =
        match name with
        | "type" -> "type'"
        | "open" -> "open'"
        | "inline" -> "inline'"
        | "checked" -> "checked'"
        | name -> name

    let getTypeType (type': string) =
        match type' with
        | "boolean" -> "bool"
        | "void" -> "unit"
        | "number" -> "float"
        | "" -> "string array"
        | "PlaybackDirection" -> "string"
        | "FillMode" -> "string"
        | "FocusOptions" -> "{| preventScroll: bool option |}"
        | type' when type'.StartsWith('{') || type'.EndsWith('}') -> "obj"
        | type' when type'.Contains('|') -> "string"
        | type' -> type'.Replace("'", "")

    let getDefaultValue (defaultValue: string) =
        match defaultValue with
        | "..." -> "None"
        | "undefined"
        | "null"
        | null -> "None"
        | value -> $"""Some {value.Replace("'", "\"")}"""

    let getSlPropTpl (prop: SlProp) =
        let name = getPropName prop.name
        let type' = getTypeType prop.``type``

        let description =
            HttpUtility.HtmlEncode(prop.description.Replace("\n", "\n    /// "))

        $"""    /// <summary>{description}</summary>
    abstract member {name} : {type'} with get, set"""


    let private getEventList (events: SlEvents array) (padding: int option) =
        let padding = defaultArg padding 0
        let padding = " " |> String.replicate padding

        let getEvents =
            events
            |> Array.fold
                (fun (current: string) next ->
                    let description =
                        HttpUtility.HtmlEncode(next.description.Replace("\n", "\n    /// "))

                    let eventDetails =
                        match next.details with
                        | "void" -> "unit"
                        | rest -> rest

                    let current =
                        if current.Length > 0 then
                            $"{current}\n"
                        else
                            ""

                    $"{padding}{current}{padding}/// - `{next.name}`: {description}")
                ""

        $"""{padding}/// Events:
{padding}{getEvents.Trim()}"""

    let private getSlotList (slots: SlSlots array) (padding: int option) =
        let padding = defaultArg padding 0
        let padding = " " |> String.replicate padding

        let getSlots =
            slots
            |> Array.fold
                (fun (current: string) next ->
                    let description =
                        HttpUtility.HtmlEncode(next.description.Replace("\n", "\n    /// "))

                    let name =
                        if next.name.Length = 0 then
                            "default"
                        else
                            next.name

                    let current =
                        if current.Length > 0 then
                            $"{current}\n"
                        else
                            ""

                    $"{padding}{current}{padding}/// - `{name}`: {description}")
                ""

        $"""{padding}/// Slots:
{padding}{getSlots.Trim()}"""


    let getComponentCommentTpl (comp: SlComponent) (padding: int option) =
        let spacePadding = defaultArg padding 0
        let padding = " " |> String.replicate spacePadding

        $"""
{padding}/// <summary>
{padding}/// Tag: {comp.tag}
{padding}///
{padding}/// Since: {comp.since}
{padding}///
{padding}/// Status: {comp.status}.
{padding}///
{padding}/// File: {comp.file}
{padding}///
{getEventList comp.events (Some spacePadding)}
{padding}///
{getSlotList comp.slots (Some spacePadding)}
{padding}///
{padding}/// </summary>"""


    let slMethodTpl (method: SlMethod) =
        let name = method.name

        let description =
            HttpUtility.HtmlEncode(method.description.Replace("\n", "\n    /// "))

        let prams =
            method.``params``
            |> Array.fold
                (fun (current: string) (next: {| isOptional: option<bool>
                                                 name: string
                                                 ``type``: string |}) ->
                    let current =
                        if current.Length > 0 then
                            $"{current}"
                        else
                            ""

                    let type' = getTypeType next.``type``

                    let isOption =
                        match next.isOptional |> Option.defaultValue false with
                        | true -> " option"
                        | false -> ""


                    $"{current} {type'}{isOption} ->")
                ""

        $"""    /// <summary>{description}</summary>
    abstract member {name} : {prams} unit"""

    let slPropAttrTpl (prop: SlProp) =
        let name = getAttrName prop.name
        let type' = getTypeType prop.``type``

        let description =
            HttpUtility.HtmlEncode(prop.description.Replace("\n", "\n    /// "))

        $"""    /// <summary>{description}</summary>
    {name} : {type'} option"""


    let private getProps (props: SlProp array) =
        props
        |> Array.fold (fun (current: string) (next: SlProp) -> $"{current}\n{getSlPropTpl next}") ""

    let private getAttrs (props: SlProp array) =
        props
        |> Array.fold (fun (current: string) (next: SlProp) -> $"{current}\n{slPropAttrTpl next}") ""

    let private getMethods (methods: SlMethod array) =
        methods
        |> Array.fold (fun (current: string) (next: SlMethod) -> $"{current}\n{slMethodTpl next}") ""

    let private getAttrRecordMemberValue (prop: SlProp) =
        let name = getAttrName prop.name
        let type' = getTypeType prop.``type``
        let value = getDefaultValue prop.defaultValue

        let addDot =
            if type' = "float"
               && value <> "None"
               && value <> "Some Fable.Core.JS.Infinity" then
                "."
            else
                ""

        $"{name} = {value}{addDot}"


    let getWithFunction (prop: SlProp) (attrsName: string) =
        let name = prop.name
        let propAttr = getAttrName prop.name
        let type' = getTypeType prop.``type``

        let valuesComment =
            let values =
                (prop.values |> Option.defaultValue [||])

            let description =
                HttpUtility.HtmlEncode(prop.description.Replace("\n", "\n    /// "))

            $"\n    /// <summary>{description}\n    /// Default Value: {prop.defaultValue}\n    /// Type: {prop.``type``}</summary>"

        $"""{valuesComment}
    let with{name.[0] |> System.Char.ToUpper}{name.[1..]} ({propAttr}: {type'}) (attrs: {attrsName}) =
        {{ attrs with {propAttr} = Some {propAttr} }}"""

    let getAttrModule (className: string) (comps: SlProp array) =
        let getAttrs =
            comps
            |> Array.fold (fun current next -> $"{current}\n        {getAttrRecordMemberValue next}") ""

        let withFunctions =
            let attrName = $"{className}Attributes"

            comps
            |> Array.fold (fun current next -> $"{current}\n    {getWithFunction next attrName}") ""

        $"""
///
[<RequireQualifiedAccess>]
module {className}Attributes  =
    let create(): {className}Attributes = {{ {getAttrs}
    }}
    {withFunctions}"""

    let getCompModule (tagAndName: string * string) (comp: SlProp array) =
        let (tag, className) = tagAndName

        let getBindingsTpl =
            let props =
                comp
                |> Array.fold
                    (fun current next ->
                        let name = getAttrName next.name
                        $"{current}\n        let {name} = attrs .> (fun attrs -> attrs.{name})")
                    ""

            let bindings =
                comp
                |> Array.fold
                    (fun (current: string) next ->
                        let name = getAttrName next.name

                        let tag =
                            next.attribute |> Option.defaultValue next.name

                        let current =
                            if current.Length > 0 then
                                $"{current}\n              "
                            else
                                ""

                        $"{current}Bind.attr(\"{tag}\", {name})")
                    ""

            if props.Length > 0 then
                $"""
    /// Provides all of the bindings available for the HTML element
    /// It leverages "Bind.attr("attribute-name", observable)" to provide reactive changes
    let stateful (attrs: IStore<{className}Attributes>) (nodes: NodeFactory seq) =
        {props}
        stateless
            [ {bindings}
              yield! nodes ]"""
            else
                ""

        $"""
///
[<RequireQualifiedAccess>]
module {className} =
    /// doesn't provide any binding helper logic and allows the user to take full
    /// control over the HTML Element either to create static HTML or do custom bindings
    /// via "bindFragment" or "Bind.attr("", binding)"
    let stateless (content: NodeFactory seq) = Html.custom("{tag}", content)
    {getBindingsTpl}"""

    let getComponentTpl (comp: SlComponent) =
        let moduleName = comp.className.[2..]
        let props = getProps comp.props
        let attrs = getAttrs comp.props
        let methods = getMethods comp.methods

        let attrsTpl =
            if comp.props.Length > 0 then
                $"\n///\ntype {comp.className}Attributes = {{ {attrs}\n}}"
            else
                ""

        let attrsModule =
            if comp.props.Length > 0 then
                getAttrModule comp.className comp.props
            else
                ""

        $"""
module Sutil.Shoelace.{moduleName}
open Browser.Types
open Sutil
open Sutil.DOM{getComponentCommentTpl comp None}
[<AllowNullLiteral>]
type {comp.className} =
    inherit HTMLElement
    {props}
    {methods}
    {attrsTpl}{attrsModule}{getCompModule (comp.tag, comp.className) (comp.props)}"""


    let getStatefulComponentTpl (comp: SlComponent) =
        if comp.props.Length > 0 then
            $"""{getComponentCommentTpl comp (Some 4)}
    static member inline {comp.className} (attrs: IStore<{comp.className}Attributes>, content: NodeFactory seq) =
        {comp.className}.stateful attrs content"""
        else
            ""

    let getStatelessComponentTpl (comp: SlComponent) =
        $"""{getComponentCommentTpl comp (Some 4)}
    static member inline {comp.className} (content: NodeFactory seq) =
        {comp.className}.stateless content"""

    let getShoelaceAPIClass (components: SlComponent array) =
        let opens =
            components
            |> Array.fold
                (fun (current: string) next ->
                    let name = next.className.[2..]

                    let current =
                        if current.Length > 0 then
                            $"{current}\n"
                        else
                            ""

                    $"{current}open Sutil.Shoelace.{name}")
                ""

        let methods =
            components
            |> Array.fold
                (fun (current: string) next ->
                    let current =
                        if current.Length > 0 then
                            $"{current}\n    "
                        else
                            ""

                    let stateful = getStatefulComponentTpl next

                    let stateless = getStatelessComponentTpl next

                    $"{current}{stateless}{stateful}")
                ""

        $"""namespace Sutil.Shoelace
open Sutil
open Sutil.DOM
{opens}
/// <summary>
/// Provides a simple API to access all Shoelace Components available
/// </summary>
type Shoelace =
    {methods}"""


    let getFsFileReference (components: SlComponent array) =
        components
        |> Array.fold
            (fun (current: string) (next: SlComponent) ->
                $"{current}\n    <Compile Include=\"{next.className.[2..]}.fs\"/>")
            """<Content Include="*.fsproj; *.fs; *.js;" Exclude="**\*.fs.js" PackagePath="fable\" />"""

    let getFsProjTpl (comps: string) (version: string) =
        $"""<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <Description>
        Sutil bindings for Shoelace Web Components.
        The contents of this package are auto-generated
    </Description>
    <PackageProjectUrl>https://github.com/AngelMunoz/Sutil.Generator</PackageProjectUrl>
    <RepositoryUrl>https://github.com/AngelMunoz/Sutil.Generator</RepositoryUrl>
    <PackageIconUrl></PackageIconUrl>
    <PackageTags>fsharp;fable;svelte</PackageTags>
    <Authors>Angel D. Munoz</Authors>
    <Version>{version}</Version>
    <PackageVersion>{version}</PackageVersion>
    <TargetFramework>netstandard2.0</TargetFramework>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
    <DefineConstants>$(DefineConstants);FABLE_COMPILER;</DefineConstants>
  </PropertyGroup>
  <PropertyGroup>
    <NpmDependencies>
      <NpmPackage Name="@shoelace-style/shoelace" Version="{version}" />
    </NpmDependencies>
  </PropertyGroup>
  <ItemGroup>
    {comps}
    <Compile Include="Library.fs" />
  </ItemGroup>
  <ItemGroup>
    <PackageReference Include="Fable.Browser.Dom" Version="2.*" />
    <PackageReference Include="Fable.Core" Version="3.*" />
    <PackageReference Include="Sutil" Version="1.0.0-*" />
  </ItemGroup>
</Project>{'\n'}"""
