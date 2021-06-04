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
