module Compiler

open System.Collections.Generic
open Grammar

type ConstTable = IDictionary<int, Literal>

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

let rec compileLiteral (literal: Literal) (table: ConstTable) : string =

    let key =
        table
        |> Seq.tryFind (fun kvp -> kvp.Value = literal)
        |> Option.map (fun kvp -> kvp.Key)

    match key with
    | Some constIndex -> $"PUSH {constIndex}"
    | None -> failwith $"Literal not found in constant table: {literal}"

let rec compileExpression e (table: ConstTable) : string =
    match e with
    | BinaryOp(op, left, right) ->
        let leftInstrs = compileExpression left table
        let rightInstrs = compileExpression right table

        let opInstr =
            match op with
            | Multiply -> "MUL"
            | Divide -> "DIV"
            | Add -> "ADD"
            | Subtract -> "SUB"

        [ leftInstrs; rightInstrs; opInstr ] |> String.concat "\n"

    | Literal literal -> compileLiteral literal table

let compileStatement (statement: Statement) table : string =
    match statement with
    | Print e -> compileExpression e table

let constTableToString (constTable: ConstTable) : string =
    constTable
    //
    |> Seq.map (fun kvp -> $".const {kvp.Key} {(formatLiteral kvp.Value)}")
    |> String.concat "\n"

let rec allExpressionLiterals (expr: Expression) : Literal list =
    match expr with
    | Literal literal -> [ literal ]
    | BinaryOp(_, left, right) -> [ left; right ] |> List.collect allExpressionLiterals

let allLiterals (statements: Statement list) : Literal list =
    statements
    |> List.collect (fun l ->
        match l with
        | Print e -> allExpressionLiterals e)

let compile (program: Statement list) : string =
    printf "%A\n" program
    let literals = allLiterals program
    let constTable = buildConstantTable (collectConstantsFromList literals)

    let statements = program |> List.map (fun x -> (compileStatement x constTable))

    let output = (constTableToString constTable) :: statements

    output |> String.concat "\n\n"
