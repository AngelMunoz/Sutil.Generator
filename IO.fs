namespace Shoelace.Generator

open FSharp.Control.Tasks
open System

open type System.Text.Encoding

open System.IO
open System.Text.Json
open System.Text.Json.Serialization
open Shoelace.Generator.Types
open CliWrap


module IO =

    let private getBytesFromStr (strval: string) =
        let b = UTF8.GetBytes(strval)

        ReadOnlySpan b

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
        use file = File.Create path

        let bytes =
            getBytesFromStr (Templates.getComponentTpl comp)

        file.Write bytes

    let private tryWriteLibraryFsProj (root: string) (version: string) (components: SlComponent array) =
        let library = Path.Combine(root, "Library.fs")

        let fsproj =
            Path.Combine(root, "Sutil.Shoelace.fsproj")

        use library = File.CreateText library
        library.WriteLine "namespace Sutil.Shoelace\n"
        library.WriteLine "type Shoelace = class end"

        use fsproj = File.Create fsproj

        let writeComponents = Templates.getFsFileReference components

        let bytes =
            getBytesFromStr (Templates.getFsProjTpl writeComponents version)

        fsproj.Write bytes


    let tryWriteFiles (version: string) (components: SlComponent array) =
        let path = Path.Combine("./", "Sutil.Shoelace")

        let dir = Directory.CreateDirectory(path)

        components
        |> Array.Parallel.iter (tryWriteComponentFile dir.FullName)

        tryWriteLibraryFsProj dir.FullName version components
