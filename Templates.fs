[<RequireQualifiedAccess>]
module Shoelace.Generator.Templates

open Shoelace.Generator.Types

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
    | "void" -> "bool"
    | "number" -> "float"
    | type' -> type'.Replace("'", "")

let getDefaultValue (defaultValue: string) =
    match defaultValue with
    | "..." -> "Some Fable.Core.JS.Infinity"
    | "undefined"
    | "null"
    | null -> "None"
    | value -> $"""Some {value.Replace("'", "\"")}"""

let getSlPropTpl (prop: SlProp) =
    let name = getPropName prop.name
    let type' = getTypeType prop.``type``

    $"""    /// <summary>{prop.description.Replace("\n", "\n    /// ")}</summary>
    abstract member {name} : {type'} with get, set
    """

let slPropAttrTpl (prop: SlProp) =
    let name = getAttrName prop.name
    let type' = getTypeType prop.``type``

    $"""    /// <summary>{prop.description.Replace("\n", "\n    /// ")}</summary>
    {name} : {type'} option"""

let private getProps (props: SlProp array) =
    props
    |> Array.fold (fun (current: string) (next: SlProp) -> $"{current}\n{getSlPropTpl next}") ""

let private getAttrs (props: SlProp array) =
    props
    |> Array.fold (fun (current: string) (next: SlProp) -> $"{current}\n{slPropAttrTpl next}") ""

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

let getAttrModule (className: string) (comps: SlProp array) =
    let getAttrs =
        comps
        |> Array.fold (fun current next -> $"{current}\n        {getAttrRecordMemberValue next}") ""

    $"""
[<RequireQualifiedAccess>]
module {className}Attributes  =
    let create(): {className}Attributes = {{ {getAttrs}
    }}
     """


let getComponentTpl (comp: SlComponent) =
    let moduleName = comp.className.[2..]
    let props = getProps comp.props
    let attrs = getAttrs comp.props

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
open Sutil.DOM
/// <summary>
/// {comp.tag} - {comp.status}.
/// File: {comp.file}
/// </summary>
type {comp.className} =
    inherit HTMLElement
    {props}
    {attrsTpl}
///{attrsModule}
    """


let getFsFileReference (components: SlComponent array) =
    components
    |> Array.fold
        (fun (current: string) (next: SlComponent) ->
            $"{current}\n    <Compile Include=\"{next.className.[2..]}.fs\"/>")
        """<Content Include="*.fsproj; *.fs; *.js;" Exclude="**\*.fs.js" PackagePath="fable\" />"""

let getFsProjTpl (comps: string) (version: string) =
    $"""<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>netcoreapp3.1</TargetFramework>
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
    <PackageReference Include="Fable.Browser.Dom" Version="2.4.4" />
    <PackageReference Include="Fable.Core" Version="3.2.7" />
    <PackageReference Include="Sutil" Version="1.0.0-*" />
  </ItemGroup>
</Project>{'\n'}"""
