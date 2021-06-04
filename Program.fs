// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System
open FSharp.Control.Tasks
open Shoelace.Generator.IO

[<EntryPoint>]
let main argv =
    task {
        let! result = downloadPackage ()

        if result <> 0 then
            raise (Exception("Failed to Download the package"))

        let! metadata = tryParseMetadata ()

        match metadata with
        | Some metadata ->
            let name = metadata.name
            let version = metadata.version
            printfn "Downloaded %s, version %s" name version
            tryWriteFiles version metadata.components
            return 0
        | None -> return 1
    }
    |> Async.AwaitTask
    |> Async.RunSynchronously
