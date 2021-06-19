namespace Sutil.Generator.Fast


(*
    DO NOT USE FANTOMAS OR ANOTHER FORMATTER 
    THAT CHANGES THE MULTI-LINE STRING STRUCTURE ON THIS FILE
    IT WILL BREAK THE DOC COMMENTS GENERATION
*)


open System
open Sutil.Generator.Types
open System.Web
open System.Text.RegularExpressions


type FastPackageJson =
    FSharp.Data.JsonProvider<"node_modules/@microsoft/fast-components/package.json", InferTypesFromValues=false>

[<RequireQualifiedAccessAttribute>]
module Templates =
    let private getUpercasedName (name: string) (includefirstWord: bool) (cammelCase: bool) =
        
        let inline pascalCase (i: int) (word: string) = 
            if i = 0 && cammelCase then 
                word.[0]
            else 
                Char.ToUpperInvariant(word.[0])

        let upercased =
            name.Split('-')
        let upercased = 
            if includefirstWord then 
                upercased |> Array.mapi (fun i word -> $"{pascalCase i word}{word.[1..]}")
            else 
                upercased |> Array.tail |> Array.mapi (fun i word -> $"{pascalCase i word}{word.[1..]}")
        String.Join("", upercased)

    let getAttributeName (name: string) =
        match name with 
        | "type" -> "``type``"
        | "open" -> "``open``"
        | "inline" -> "``inline``"
        | "checked" -> "``checked``"
        | rest when rest.Contains('-') ->
            getUpercasedName rest true true
        | rest -> rest

    let getAttributeType (type': string) =
        match type' with
        | "boolean" -> "bool"
        | "void" -> "unit"
        | "number" -> "float"
        | type' when type'.StartsWith('{') || type'.EndsWith('}') -> "obj"
        | type' -> type'

    let getDefaultValue (value: string) = 
        match value with
        | "undefined"
        | "null"
        | ""
        | null -> "None"
        | value ->
            let regex = new Regex(@"^[+-]?(([1-9][0-9]*)?[0-9](\.[0-9]*)?|\.[0-9]+)$")
            match value with
            | "true"
            | "false" ->
                $"""Some {value}"""
            | value when regex.IsMatch (value) -> 
                $"""Some {value}"""
            | value -> $"""Some "{value}" """

    let getDescription (description: string option) =
        match description with 
        | Some description ->
            HttpUtility.HtmlEncode(description.Replace("\n", "\n    /// "))
        | None -> ""


    let private getFastAttr (prop: AttributeVscodeDefinition) =
        let name = getAttributeName prop.name
        let type' = getAttributeType prop.``type``
        
        let description = getDescription prop.description

        $"""    /// <summary>{description}</summary>
    abstract member {name} : {type'} with get, set"""
    let private getFastElementAttributes (props: AttributeVscodeDefinition seq) =
        props
        |> Seq.fold (fun (current: string) (next: AttributeVscodeDefinition) -> $"{current}\n{getFastAttr next}") ""

    let private getFastAttributesTypeTpl (prop: AttributeVscodeDefinition) =
        let name = getAttributeName prop.name
        let type' = getAttributeType prop.``type``

        let description = getDescription prop.description

        $"""    /// <summary>{description}</summary>
    {name} : {type'} option"""

    let private getAttrs (props: AttributeVscodeDefinition seq) =
        props
        |> Seq.fold (fun (current: string) (next: AttributeVscodeDefinition) -> $"{current}\n{getFastAttributesTypeTpl next}") ""


    let getSlotList (slots: SlotVsCodeDefinition seq) (padding: int option) = 
        let padding = defaultArg padding 0
        let padding = " " |> String.replicate padding

        let getSlots =
            slots
            |> Seq.fold
                (fun (current: string) next ->
                    let description = getDescription next.description

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

    let getComponentCommentTpl (comp: TagVsCodeDefinition) (padding: int option) =
        let spacePadding = defaultArg padding 0
        let padding = " " |> String.replicate spacePadding
        $"""
{padding}/// <summary>
{padding}/// Title: {comp.title}
{padding}///
{padding}/// Tag: {comp.name}
{padding}///
{getSlotList comp.slots (Some spacePadding)}
{padding}///
{padding}/// </summary>"""

    let getCompModule (comp: TagVsCodeDefinition) (attrs: AttributeVscodeDefinition seq) = 
        let tag = comp.name
        let name = getUpercasedName comp.name true false

        let getBindingsTpl =
            let props =
                attrs
                |> Seq.fold
                    (fun current next ->
                        let name = getAttributeName next.name
                        $"{current}\n        let {name} = attrs .> (fun attrs -> attrs.{name})")
                    ""

            let bindings =
                attrs
                |> Seq.fold
                    (fun (current: string) next ->
                        let name = getAttributeName next.name

                        let tag =
                            next.name

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
    let stateful (attrs: IStore<{name}Attributes>) (nodes: NodeFactory seq) =
        {props}
        stateless
            [ {bindings}
              yield! nodes ]"""
            else
                ""
        $"""
///
[<RequireQualifiedAccess>]
module {name} =
    /// doesn't provide any binding helper logic and allows the user to take full
    /// control over the HTML Element either to create static HTML or do custom bindings
    /// via "bindFragment" or "Bind.attr("", binding)"
    let stateless (content: NodeFactory seq) = Html.custom("{tag}", content)
    {getBindingsTpl}"""


    let getWithFunction (prop: AttributeVscodeDefinition) (attrsName: string) =
        let name = getUpercasedName prop.name true false
        let propAttr = getAttributeName prop.name
        let type' = getAttributeType prop.``type``

        let valuesComment =
            let values =
                (prop.values |> Option.defaultValue Seq.empty)

            let description = getDescription prop.description

            $"\n    /// <summary>{description}\n    /// Default Value: {prop.``default``}\n    /// Type: {prop.``type``}</summary>"

        $"""{valuesComment}
    let with{name} ({propAttr}: {type'}) (attrs: {attrsName}) =
        {{ attrs with {propAttr} = Some {propAttr} }}"""

    let private getAttrRecordMemberValue (prop: AttributeVscodeDefinition) =
        let name = getAttributeName prop.name
        let type' = getAttributeType prop.``type``
        let defValue = 
            prop.``default`` |> Option.defaultValue ("undefined" :> obj)
        let value = getDefaultValue (defValue.ToString().ToLowerInvariant())

        let addDot =
            match type' with 
            | "float" when value.Contains('.') && value <> "None" -> ""
            | "float" when value <> "None" -> "."
            | _ -> ""
        
        let value = 
            match value with 
            | "Some true" when type' = "string" -> "Some \"true\""
            | "Some false" when type' = "string" -> "Some \"false\""
            | value -> value

        $"{name} = {value}{addDot}"

    let getAttrModule (name: string) (attrs: AttributeVscodeDefinition seq) =
        let getAttrs =
            attrs
            |> Seq.fold (fun current next -> $"{current}\n        {getAttrRecordMemberValue next}") ""

        let withFunctions =
            let attrName = $"{name}Attributes"

            attrs
            |> Seq.fold (fun current next -> $"{current}\n    {getWithFunction next attrName}") ""

        $"""
///
[<RequireQualifiedAccess>]
module {name}Attributes  =
    let create(): {name}Attributes = {{ {getAttrs}
    }}
    {withFunctions}"""

    let getComponentTpl (comp: TagVsCodeDefinition) =
        let moduleName = getUpercasedName comp.name true false
        let props = getFastElementAttributes comp.attributes
        let attrs = getAttrs comp.attributes
        let hasAttributes = comp.attributes |> Seq.length > 0
        let attrsTpl =
            if hasAttributes then
                $"\n///\ntype {getUpercasedName comp.name true false}Attributes = {{ {attrs}\n}}"
            else
                ""

        let attrsModule =
            if hasAttributes then
                getAttrModule (getUpercasedName comp.name true false) comp.attributes
            else
                ""

        $"""
module Sutil.Fast.{moduleName}
open Browser.Types
open Sutil
open Sutil.DOM
{getComponentCommentTpl comp None}
[<AllowNullLiteral>]
type {getUpercasedName comp.name true false} =
    inherit HTMLElement
    {props}
    {attrsTpl}{attrsModule}
    {getCompModule comp comp.attributes}"""


    let getStatefulComponentTpl (comp: TagVsCodeDefinition) =
        if comp.attributes |> Seq.length > 0 then
            let name = getUpercasedName comp.name true false
            $"""{getComponentCommentTpl comp (Some 4)}
    static member inline {name} (attrs: IStore<{name}Attributes>, content: NodeFactory seq) =
        {name}.stateful attrs content"""
        else
           ""

    let getStatelessComponentTpl (comp: TagVsCodeDefinition) =
        let name = getUpercasedName comp.name true false
        $"""{getComponentCommentTpl comp (Some 4)}
    static member inline {name} (content: NodeFactory seq) =
        {name}.stateless content"""

    let getFastAPIClass (components: TagVsCodeDefinition array) =
        let opens =
            components
            |> Array.fold
                (fun (current: string) next ->
                    let name = getUpercasedName next.name true false

                    let current =
                        if current.Length > 0 then
                            $"{current}\n"
                        else
                            ""

                    $"{current}open Sutil.Fast.{name}")
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

        $"""namespace Sutil.Fast
open Sutil
open Sutil.DOM
{opens}
/// <summary>
/// Provides a simple API to access all Shoelace Components available
/// </summary>
type Fast =
    {methods}"""


    let getFsFileReference (components: TagVsCodeDefinition array) =

        components
        |> Array.fold
            (fun (current: string) (next: TagVsCodeDefinition) ->
                $"{current}\n    <Compile Include=\"{getUpercasedName next.name false false}.fs\"/>")
            """<Content Include="*.fsproj; *.fs; *.js;" Exclude="**\*.fs.js" PackagePath="fable\" />"""

    let getFsProjTpl (comps: string) (package: string) (version: string) =
        $"""<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <Description>
        Sutil bindings for {package} Web Components.
        The contents of this package are auto-generated
    </Description>
    <PackageProjectUrl>https://github.com/AngelMunoz/Sutil.Generator</PackageProjectUrl>
    <RepositoryUrl>https://github.com/AngelMunoz/Sutil.Generator</RepositoryUrl>
    <PackageIconUrl></PackageIconUrl>
    <PackageTags>fsharp;fable;svelte</PackageTags>
    <Authors>Angel D. Munoz</Authors>
    <Version>{version}-beta</Version>
    <PackageVersion>{version}-beta</PackageVersion>
    <TargetFramework>netstandard2.0</TargetFramework>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
    <DefineConstants>$(DefineConstants);FABLE_COMPILER;</DefineConstants>
  </PropertyGroup>
  <PropertyGroup>
    <NpmDependencies>
      <NpmPackage Name="{package}" Version="{version}" />
      <NpmPackage Name="lodash-es" Version="4.17.21" />
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
