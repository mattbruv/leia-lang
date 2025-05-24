module Program

open System.Collections.Generic
open System.IO
open Grammar
open Parser

let program =
    many whitespaceChar >>. sepBy1 pexpression whitespace1
    .>> many whitespaceChar
    .>> eof

let contents = File.ReadAllText("examples/syntax.leia")
let result = run program contents

let rec collectConstants (constants: HashSet<Literal>) (lit: Literal)  =
    match lit with
    | Int _ | Float _ | Boolean _ | LString _ ->
        constants.Add(lit) |> ignore
    | BinaryOp(_, left, right) ->
        collectConstants constants left
        collectConstants constants right
    | Identifier _ -> ()

let collectConstantsFromList (lits: Literal list) =
    let constants = HashSet()
    lits |> List.iter (collectConstants constants)
    constants

let buildConstantTable (constants: seq<Literal>) =
    constants
    |> Seq.mapi (fun idx lit -> idx, lit)
    |> dict

let rec compile (literal: Literal): string =
    match literal with
    | Boolean a -> a.ToString()
    | Int i ->  i.ToString()
    | Float f -> f.ToString()
    | LString s -> s.ToString()
    | Identifier s -> s.ToString()
    | BinaryOp(binaryOp, left, right) -> "( " + compile left +  " " + binaryOp.ToString() + " " + compile right + " )"

match result with
    | Success (literals, _ ) ->
        let consts = HashSet()
        literals |> List.map (collectConstants consts) |> ignore
        let table = buildConstantTable consts
        table
        |> Seq.iter (fun constant -> printfn $".const %A{constant.Key} %A{constant.Value}")
    | Failure(s, s1, parserPosition) -> printResult result

