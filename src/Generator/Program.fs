// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System.Threading.Tasks
open FSharp.Control.Tasks
open Sutil.Generator.Generation
open Argu
open Sutil.Generator.Types



[<EntryPoint>]
let main argv =

    let parser =
        ArgumentParser.Create<Args>(programName = "Sutil.Generator")

    let result =
        parser.Parse(argv, ignoreUnrecognized = true)

    let activeTask () =
        match result.GetAllResults() with
        | [ Component_System ComponentSystem.Fast ] ->
            task {
                do! generateLibrary ComponentSystem.Fast
                return 0
            }
        | [ Component_System ComponentSystem.Shoelace ]
        | _ ->
            task {
                do! generateLibrary ComponentSystem.Shoelace
                return 0
            }

    activeTask ()
    |> Async.AwaitTask
    |> Async.RunSynchronously
