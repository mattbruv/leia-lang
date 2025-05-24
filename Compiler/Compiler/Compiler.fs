module Compiler

open System.Collections.Generic
open Grammar
open Microsoft.FSharp.Collections
open Opcodes

type ConstTable = IDictionary<int, Literal>
type LocalMap = Map<string, int>

type CompilerEnv =
    { locals: LocalMap
      constTable: ConstTable }

type Emitted =
    | Instruction of Opcode * string option
    | Label of string
    | Comment of string

let emit opcode : Emitted = Instruction(opcode, None)
let emitWithComment (opcode, comment) = Instruction(opcode, comment)

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

let rec compileLiteral (literal: Literal) (env: CompilerEnv) : Emitted list =

    match literal with
    | Identifier ident ->
        let maybeEntry =
            env.locals |> Seq.tryFind (fun kvp -> kvp.Key = ident) |> Option.map id

        match maybeEntry with
        | Some entry -> [ emit (LoadLocal entry.Value) ]
        | None -> failwith $"Identifier not found in table: {ident}"

    | _ ->
        let maybeEntry =
            env.constTable |> Seq.tryFind (fun kvp -> kvp.Value = literal) |> Option.map id

        match maybeEntry with
        | Some entry -> [ emitWithComment ((PushConstant entry.Key), Some(formatLiteral entry.Value)) ]
        | None -> failwith $"Literal not found in constant table: {literal}"

let rec compileExpression e (env: CompilerEnv) : Emitted list =
    match e with
    | BinaryOp(op, left, right) ->
        let leftInstrs = compileExpression left env
        let rightInstrs = compileExpression right env

        let opInstr =
            match op with
            | Multiply -> emit Mul
            | Divide -> emit Div
            | BinaryOp.Add -> emit Add
            | Subtract -> emit Sub
            | Modulo -> emit Mod

        leftInstrs @ rightInstrs @ [ opInstr ]

    | Literal literal -> compileLiteral literal env
    | Assignment(s, expression) -> [ emit (LoadLocal 1) ] @ compileExpression expression env

let compileStatement (statement: Statement) env : Emitted list =
    match statement with
    | Statement.Print e -> (compileExpression e env) @ [ emit Print ]
    | Statement.Expr e -> (compileExpression e env)

let constTableToString (constTable: ConstTable) : string =
    constTable
    //
    |> Seq.map (fun kvp -> $".const {kvp.Key} {(formatLiteral kvp.Value)}")
    |> String.concat "\n"

let rec allExpressionLiterals (expr: Expression) : Literal list =
    match expr with
    | Literal literal -> [ literal ]
    | BinaryOp(_, left, right) -> [ left; right ] |> List.collect allExpressionLiterals
    | Assignment(_, expression) -> allExpressionLiterals expression

let allLiterals (statements: Statement list) : Literal list =
    statements
    |> List.collect (fun l ->
        match l with
        | Statement.Print e -> allExpressionLiterals e
        | Statement.Expr e -> allExpressionLiterals e)

let emittedToString emitted =
    match emitted with
    | Label s -> "." + s
    | Comment s -> "; " + s
    | Instruction(opcode, stringOption) ->
        let op =
            match opcode with
            | PushConstant i -> "PUSH " + i.ToString()
            | Call s -> "CALL " + s
            | Return -> "RET"
            | Jump s -> "JUMP " + s
            | JumpIfZero s -> "JUMPZ " + s
            | JumpIfNotZero s -> "JUMPNZ " + s
            | Equals -> "EQ"
            | GreaterThan -> "GT"
            | Add -> "ADD"
            | Sub -> "SUB"
            | Mul -> "MUL"
            | Div -> "DIV"
            | Mod -> "MOD"
            | Print -> "PRINT"
            | Halt -> "HALT"
            | LoadLocal i -> "LOAD " + i.ToString()
            | StoreLocal i -> "STORE " + i.ToString()

        match stringOption with
        | Some value -> op + " ; " + value
        | None -> op

let compile (program: Statement list) : string =
    printf "%A\n" program
    let literals = allLiterals program
    let constTable = buildConstantTable (collectConstantsFromList literals)

    let env: CompilerEnv =
        { locals = Map.empty
          constTable = constTable }

    let statements = program |> List.collect (fun x -> (compileStatement x env))

    let main = Label "main"
    let halt = emit Halt

    let programEmitted: Emitted list = (main :: statements) @ [ halt ]

    let main =
        programEmitted
        //
        |> List.map emittedToString
        |> String.concat "\n"

    let output = (constTableToString constTable) + "\n\n" + main
    output
