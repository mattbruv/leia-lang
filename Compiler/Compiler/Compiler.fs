module Compiler

open System.Collections.Generic
open Grammar

let formatLiteral literal =
    match literal with
    | Int i -> i.ToString()
    | Float f -> f.ToString()
    | LString s -> $"\"{s}\""
    | Boolean b -> b.ToString()
    | Identifier s -> s

let rec collectConstants (constants: HashSet<Literal>) (lit: Literal) =
    match lit with
    | Int _
    | Float _
    | Boolean _
    | LString _ -> constants.Add(lit) |> ignore
    | Identifier _ -> ()

let collectConstantsFromList (lits: Literal list) =
    let constants = HashSet()
    lits |> List.iter (collectConstants constants)
    constants

let buildConstantTable (constants: seq<Literal>) =
    constants |> Seq.mapi (fun idx lit -> idx, lit) |> dict

let rec compileLiteral (literal: Literal) : string =
    match literal with
    | Boolean a -> a.ToString()
    | Int i -> i.ToString()
    | Float f -> f.ToString()
    | LString s -> s.ToString()
    | Identifier s -> s.ToString()

let constTableToString (constTable: IDictionary<int, Literal>) : string =
    constTable
    //
    |> Seq.map (fun kvp -> $".const {kvp.Key} {(formatLiteral kvp.Value)}")
    |> String.concat "\n"

let allLiterals (statements: Statement list) =
    statements
    |> List.map (fun l ->
        match l with
        | Print e ->
            match e with
            | Literal l -> l)

let compile (program: Statement list) : string =
    printf "%A\n" program
    let literals = allLiterals program
    let constTable = buildConstantTable (collectConstantsFromList literals)

    let output = [ (constTableToString constTable) ]

    output |> String.concat "\n\n"
