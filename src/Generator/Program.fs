// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System.Threading.Tasks
open FSharp.Control.Tasks
open Sutil.Generator.IO
open Argu
open Sutil.Generator.Types



[<EntryPoint>]
let main argv =

    let parser =
        ArgumentParser.Create<Args>(programName = "Sutil.Generator")

    let result =
        parser.Parse(argv, ignoreUnrecognized = true)

    let activeTask() = 
        match result.GetAllResults() with
        | [ Component_System ComponentSystem.Fast ] ->
            printfn "Start FAST pipeline"
            Task.FromResult(0)
        | [ Component_System ComponentSystem.Shoelace ]
        | _ -> 
            task {
                do! generateLibrary ComponentSystem.Shoelace
                return 0
            }
    
    activeTask()
    |> Async.AwaitTask
    |> Async.RunSynchronously
