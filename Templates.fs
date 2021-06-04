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

let getAttrname (name: string) =
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

let getSlPropTpl (prop: SlProp) =
    let name = getPropName prop.name
    let type' = getTypeType prop.``type``

    $"""    /// <summary>{prop.description.Replace("\n", "\n    /// ")}</summary>
    abstract member {name} : {type'} with get, set
    """

let slPropAttrTpl (prop: SlProp) =
    let name = getAttrname prop.name
    let type' = getTypeType prop.``type``

    $"""    /// <summary>{prop.description.Replace("\n", "\n    /// ")}</summary>
    {name}: {type'} option"""

let private getProps (props: SlProp array) =
    props
    |> Array.fold (fun (current: string) (next: SlProp) -> $"{current}\n{getSlPropTpl next}") ""

let private getAttrs (props: SlProp array) =
    props
    |> Array.fold (fun (current: string) (next: SlProp) -> $"{current}\n{slPropAttrTpl next}") ""

let getComponentTpl (comp: SlComponent) =
    let moduleName = comp.className.[2..]
    let props = getProps comp.props
    let attrs = getAttrs comp.props

    let attrsTpl =
        if comp.props.Length > 0 then
            $"\n///\ntype {comp.className}Attributes = {{ {attrs}\n}}"
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
