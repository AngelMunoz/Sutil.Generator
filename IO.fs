namespace Shoelace.Generator

open FSharp.Control.Tasks
open System.IO
open System.Text.Json
open System.Text.Json.Serialization
open Shoelace.Generator.Types
open CliWrap


module IO =

    let downloadPackage () =
        let cmd =
            Cli
                .Wrap("npx")
                .WithArguments("pnpm install @shoelace-style/shoelace")
                .WithStandardErrorPipe(PipeTarget.ToStream(System.Console.OpenStandardError()))
                .WithStandardOutputPipe(PipeTarget.ToStream(System.Console.OpenStandardOutput()))
                .WithValidation CommandResultValidation.None

        task {
            let! result = cmd.ExecuteAsync()
            return result.ExitCode
        }

    let private getJsonOptions () =
        let opts = JsonSerializerOptions()

        opts.AllowTrailingCommas <- true
        opts.IgnoreNullValues <- true
        opts.ReadCommentHandling <- JsonCommentHandling.Skip
        opts.Converters.Add(JsonFSharpConverter())
        opts

    let tryParseMetadata () =
        task {
            try
                let path =
                    let combined =
                        Path.Combine("./", "node_modules", "@shoelace-style", "shoelace", "dist", "metadata.json")

                    Path.GetFullPath combined

                use fileStr = File.OpenRead path

                let! serialized = JsonSerializer.DeserializeAsync<ShoelaceMetadata>(fileStr, getJsonOptions ())
                return Some serialized
            with ex ->
                eprintfn "%s" ex.Message
                return None
        }

    let private tryWriteComponentFile (root: string) (comp: SlComponent) =
        let name = comp.className.[2..]
        let path = Path.Combine(root, $"{name}.fs")
        use file = File.CreateText path
        file.WriteLine($"module Sutil.Shoelace.{name}")

    let private tryWriteLibraryFsProj (root: string) (version: string) (components: SlComponent array) =
        let library = Path.Combine(root, "Library.fs")

        let fsproj =
            Path.Combine(root, "Sutil.Shoelace.fsproj")

        use library = File.CreateText library
        library.WriteLine "namespace Sutil.Shoelace\n"
        library.WriteLine "type Shoelace = class end"

        use fsproj = File.Create fsproj

        let writeComponents =
            components
            |> Array.fold
                (fun (current: string) (next: SlComponent) ->
                    $"{current}\n    <Compile Include=\"{next.className.[2..]}.fs\"/>")
                """<Content Include="*.fsproj; *.fs; *.js;" Exclude="**\*.fs.js" PackagePath="fable\" />"""


        let template =
            $"""
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>netcoreapp3.1</TargetFramework>
  </PropertyGroup>
  <PropertyGroup>
    <NpmDependencies>
      <NpmPackage Name="@shoelace-style/shoelace" Version="{version}" />
    </NpmDependencies>
  </PropertyGroup>
  <ItemGroup>
    {writeComponents}
    <Compile Include="Library.fs" />
  </ItemGroup>
  <ItemGroup>
    <PackageReference Include="Fable.Browser.Dom" Version="2.4.4" />
    <PackageReference Include="Fable.Core" Version="3.2.7" />
    <PackageReference Include="Sutil" Version="1.0.0-*" />
  </ItemGroup>
</Project>"""

        let bytes =
            let b =
                System.Text.Encoding.UTF8.GetBytes(template.TrimStart())

            System.ReadOnlySpan b

        fsproj.Write bytes


    let tryWriteFiles (version: string) (components: SlComponent array) =
        let path =
            Path.Combine("./", "Shoelace.Components")

        let dir = Directory.CreateDirectory(path)

        components
        |> Array.Parallel.iter (tryWriteComponentFile dir.FullName)

        tryWriteLibraryFsProj dir.FullName version components
