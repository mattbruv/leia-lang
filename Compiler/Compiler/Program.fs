module Program

open System
open System.IO
open Parser
open Compiler

let writeOutput outputPath programText =
    match outputPath with
    | Some path -> File.WriteAllText(path, programText)
    | None ->
        Console.OutputEncoding <- System.Text.Encoding.UTF8
        printf $"%s{programText}"

[<EntryPoint>]
let main argv =
    match argv with
    | [| inputPath |] ->
        let source = File.ReadAllText(inputPath)

        match run program source with
        | Success(literals, _) ->
            let program = $"%s{compile literals}"
            writeOutput None program
            0
        | Failure _ -> 1
    | [| inputPath; outputPath |] ->
        let source = File.ReadAllText(inputPath)

        match run program source with
        | Success(literals, _) ->
            let program = $"%s{compile literals}"
            writeOutput (Some outputPath) program
            0
        | Failure _ -> 1
    | _ ->
        eprintfn "Usage: compiler.exe <input> [output]"
        1
