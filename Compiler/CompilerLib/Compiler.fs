module Compiler

open System.Collections.Generic
open Grammar
open Microsoft.FSharp.Collections
open Opcodes
open Parser
open Organize

type ConstTable = IDictionary<int, Literal>
type LocalMap = Map<string, int>

type CompilerEnv =
    { locals: LocalMap
      constTable: ConstTable
      nextSlot: int
      nextLabel: int }

let getNextLabel env : Label * CompilerEnv =
    let newLabel = env.nextLabel
    Label("L" + newLabel.ToString()), { env with nextLabel = newLabel + 1 }

let getOrAddLocal (env: CompilerEnv) (name: string) : int * CompilerEnv =
    match Map.tryFind name env.locals with
    | Some index -> index, env
    | None ->
        let newIndex = env.nextSlot
        let updatedLocals = Map.add name newIndex env.locals

        newIndex,
        { env with
            locals = updatedLocals
            nextSlot = newIndex + 1 }

type Emitted =
    | Instruction of Opcode * string option
    | FunctionLabel of Label
    | EmittedLabel of Label
    | Comment of string

let emit opcode : Emitted = Instruction(opcode, None)
let emitWithComment (opcode, comment) = Instruction(opcode, comment)

let formatLiteral literal =
    match literal with
    | Int i -> i.ToString()
    | Float f -> f.ToString()
    | LString s -> $"\"{s}\""
    | Boolean b -> b.ToString()
    | Identifier s -> Ident.value s

let rec collectConstants (constants: HashSet<Literal>) (lit: Literal) =
    match lit with
    | Int _
    | Float _
    | Boolean _
    | LString _ -> constants.Add(lit) |> ignore
    | Identifier _ -> ()

let rec collectLiterals (literals: HashSet<string>) (lit: Literal) =
    match lit with
    | Identifier ident -> literals.Add(Ident.value ident) |> ignore
    | _ -> ()

let collectConstantsFromList (lits: Literal list) =
    let constants = HashSet()
    lits |> List.iter (collectConstants constants)
    constants

let buildConstantTable (constants: seq<Literal>) : ConstTable =
    constants |> Seq.mapi (fun idx lit -> idx, lit) |> dict

let buildLocalsTable (locals: seq<string>) : LocalMap =
    locals |> Seq.rev |> Seq.mapi (fun idx ident -> ident, idx) |> Map

let getLocal env ident =
    env.locals |> Seq.tryFind (fun kvp -> kvp.Key = ident) |> Option.map id

let rec compileLiteral (literal: Literal) (env: CompilerEnv) : Emitted list * CompilerEnv =

    match literal with
    | Identifier ident ->
        match getLocal env (Ident.value ident) with
        | Some entry -> [ emitWithComment (LoadLocal entry.Value, Some entry.Key) ], env
        | None -> failwith $"Identifier not found in table: {ident}"

    | _ ->
        let maybeEntry =
            env.constTable |> Seq.tryFind (fun kvp -> kvp.Value = literal) |> Option.map id

        match maybeEntry with
        | Some entry -> [ emitWithComment ((PushConstant entry.Key), Some(formatLiteral entry.Value)) ], env
        | None -> failwith $"Literal not found in constant table: {literal}"

let opEmit op =
    match op with
    | Multiply -> emit Mul
    | Divide -> emit Div
    | BinaryOp.Add -> emit Add
    | Subtract -> emit Sub
    | Modulo -> emit Mod
    | GreaterThan -> emit Gt
    | GreaterThanEqual -> emit Gte
    | LessThan -> emit Lt
    | LessThanEqual -> emit Lte
    | NotEqual -> emit NotEq
    | Equal -> emit Eq
    | And
    | Or -> failwith $"Should not be emitting instruction {op}"

let fnLabel ident = Label.Label($"fn_{Ident.value ident}")

let rec compileExpression e (env: CompilerEnv) : Emitted list * CompilerEnv =
    match e with
    | BinaryOp(op, left, right) ->
        let leftEmit, env' = compileExpression left env
        let rightEmit, env'' = compileExpression right env'

        match op with
        | And
        | Or ->
            let jumpOp =
                match op with
                | And -> JumpIfZero
                | Or -> JumpIfNotZero
                | _ -> failwith "Unexpected logical op"

            let label_end, env3 = getNextLabel env''

            leftEmit //
            @ [ emit (jumpOp label_end); emit Pop ]
            @ rightEmit
            @ [ EmittedLabel label_end ],
            env3

        | _ -> leftEmit @ rightEmit @ [ opEmit op ], env''

    | Literal literal -> compileLiteral literal env
    | Assignment(ident, expression) ->
        let exprInstrs, env' = compileExpression expression env
        let slot, env'' = getOrAddLocal env' (Ident.value ident)

        let instrs =
            let store = [ emitWithComment (StoreLocal slot, Some $"{ident}") ]

            match expression with
            // If we are assigning something which was previously assigned, emit a load so it's on the stack again when we pull it.
            | Assignment(s, _) ->
                let childIdx, _ = getOrAddLocal env'' (Ident.value s)
                let load = [ emitWithComment (LoadLocal childIdx, Some $"load {s} again") ]
                exprInstrs @ load @ store
            // otherwise, just store it
            | _ -> exprInstrs @ store

        instrs, env''
    | Expression.Call fn ->
        // Compile function call
        // push all expression arguments onto stack in reverse order
        // we push in reverse order so that they can be popped from stack
        // and stored as function locals in the correct order
        let body, env2 =
            fn.arguments
            |> List.rev
            |> List.fold
                (fun (acc, currentEnv) stmt ->
                    let emitted, newEnv = compileExpression stmt currentEnv
                    (acc @ emitted, newEnv))
                ([], env)

        let call = body @ [ emit (Call(fnLabel fn.name)) ]
        call, env2

let rec compileDeclaration (declaration: Declaration) env : Emitted list * CompilerEnv =
    match declaration with
    | FunctionDeclaration fn -> compileFunction fn env
    | Statement statement -> compileStatement statement env

and compileFunction (fn: Function) env : Emitted list * CompilerEnv =
    // Add function label
    let label = FunctionLabel(fnLabel fn.name)

    let paramNames =
        match fn.parameters with
        | Some ps -> List.map Ident.value ps
        | None -> []

    let paramLocals = paramNames |> List.mapi (fun idx name -> name, idx) |> Map.ofList

    let envWithParams =
        { env with
            locals = paramLocals
            nextSlot = List.length paramNames }

    // pop all arguments off the stack and store them into their respective locals
    let storeParams: Emitted list =
        paramLocals //
        |> Map.toList
        |> List.sortBy snd // store in argument order which was pushed onto the stack
        |> List.map (fun (k, v) -> emitWithComment (StoreLocal v, Some $"push {k}"))

    // fn declarations should not be nested
    let decl_fns =
        fn.body
        |> List.choose (function
            | FunctionDeclaration f -> Some f
            | _ -> None)

    let decl_others =
        fn.body
        |> List.choose (function
            | FunctionDeclaration _ -> None
            | x -> Some x)

    // Add compiled body (non function declarations)
    let body, env2 =
        decl_others
        |> List.fold
            (fun (acc, currentEnv) stmt ->
                let emitted, newEnv = compileDeclaration stmt currentEnv
                (acc @ emitted, newEnv))
            ([], envWithParams)

    // Add compiled nested function declarations.
    let nested_functions, env3 =
        decl_fns
        |> List.fold
            (fun (acc, currentEnv) stmt ->
                let emitted, newEnv = compileFunction stmt currentEnv
                (acc @ emitted, newEnv))
            ([], env2)


    // If we are in the main function, we don't want to return, but rather halt
    let returnOp =
        match Ident.value fn.name with
        | "main" -> [ emit Halt ]
        | _ -> [ emit Return ]

    [ label ] @ storeParams @ body @ returnOp @ nested_functions, env3

and compileStatement (statement: Statement) env : Emitted list * CompilerEnv =
    match statement with
    | Statement.Print e ->
        let exprInstrs, env' = (compileExpression e env)
        exprInstrs @ [ emit Print ], env'
    | Statement.ExprStatement e -> (compileExpression e env)
    | If(condition, ifBody, elseBody) ->
        // if the expression is false, jump to end
        let elseStart, env2 = getNextLabel env
        let ifElseEnd, env2 = getNextLabel env2
        let conditionInstrs, env3 = compileExpression condition env2
        let bodyInstrs, env4 = compileStatement ifBody env3

        let elseInstrs, env5 =
            match elseBody with
            | Some elseStatement -> compileStatement elseStatement env4
            | None -> [], env4

        conditionInstrs
        @ [ emit (JumpIfZero elseStart) ] // Jump past if block if expression was false
        @ [ emit Pop ] // pop if condition
        @ bodyInstrs // If block
        @ [ emit (Jump ifElseEnd) ] // We have already run the If block, so jump past else
        @ [ EmittedLabel elseStart ]
        @ [ emit Pop ] // pop if condition
        @ elseInstrs // Else block
        @ [ EmittedLabel ifElseEnd ], // End of if statement
        env5

    | Block declarations ->
        declarations
        |> List.fold
            (fun (acc, currentEnv) decl ->
                let emitted, newEnv = compileDeclaration decl currentEnv
                (acc @ emitted, newEnv))
            ([], env)

let constTableToString (constTable: ConstTable) : string =
    constTable
    |> Seq.map (fun kvp -> $".const {kvp.Key} {(formatLiteral kvp.Value)}")
    |> String.concat "\n"

let rec allExpressionLiterals (expr: Expression) : Literal list =
    match expr with
    | Literal literal -> [ literal ]
    | BinaryOp(_, left, right) -> [ left; right ] |> List.collect allExpressionLiterals
    | Assignment(_, expression) -> allExpressionLiterals expression
    | Expression.Call fn -> [ Identifier fn.name ] @ List.collect allExpressionLiterals fn.arguments

let rec allDeclarationLiterals (declaration: Declaration) : Literal list =
    match declaration with
    | FunctionDeclaration fn ->

        let ps =
            match fn.parameters with
            | Some ps -> List.map Identifier ps
            | None -> []

        let body = List.collect allDeclarationLiterals fn.body

        [ Identifier fn.name ] @ ps @ body
    | Statement statement -> allStatementLiterals statement

and allStatementLiterals (statement: Statement) : Literal list =
    match statement with
    | Statement.Print e -> allExpressionLiterals e
    | Statement.ExprStatement e -> allExpressionLiterals e
    | If(e, ifBlock, elseBlock) ->
        let maybeElse =
            match elseBlock with
            | Some value -> allStatementLiterals value
            | None -> []

        allExpressionLiterals e @ allStatementLiterals ifBlock @ maybeElse
    | Block declarations -> List.collect allDeclarationLiterals declarations

let emittedToString emitted =
    match emitted with
    | FunctionLabel s -> "\n." + Label.value s
    | EmittedLabel s -> "." + Label.value s
    | Comment s -> "; " + s
    | Instruction(opcode, stringOption) ->
        let op =
            match opcode with
            | PushConstant i -> "PUSH_CONST " + i.ToString()
            | Pop -> "POP"
            | Call s -> "CALL " + Label.value s
            | Return -> "RET"
            | Jump s -> "JUMP " + Label.value s
            | JumpIfZero s -> "JUMPZ " + Label.value s
            | JumpIfNotZero s -> "JUMPNZ " + Label.value s
            | Add -> "ADD"
            | Sub -> "SUB"
            | Mul -> "MUL"
            | Div -> "DIV"
            | Mod -> "MOD"
            | Print -> "PRINT"
            | Halt -> "HALT"
            | LoadLocal i -> "LOAD_LOCAL " + i.ToString()
            | StoreLocal i -> "STORE_LOCAL " + i.ToString()
            | Eq -> "EQ"
            | NotEq -> "NEQ"
            | Gt -> "GT"
            | Gte -> "GTE"
            | Lt -> "LT"
            | Lte -> "LTE"

        let prefix = "    "

        match stringOption with
        | Some value -> prefix + op + " ; " + value
        | None -> prefix + op

let compile (program: Declaration list) : string =
    // printf "%A\n" program
    // List.map (fun x -> printfn $"{x}") program |> ignore
    let program = organize program

    let literals = List.collect allDeclarationLiterals program
    let constTable = buildConstantTable (collectConstantsFromList literals)

    let env: CompilerEnv =
        { locals = Map.empty
          constTable = constTable
          nextSlot = 0
          nextLabel = 0 }

    let programEmitted, _ =
        program
        |> List.fold
            (fun (acc, currentEnv) stmt ->
                let emitted, newEnv = compileDeclaration stmt currentEnv
                (acc @ emitted, newEnv))
            ([], env)

    let main = programEmitted |> List.map emittedToString |> String.concat "\n"

    let output = (constTableToString constTable) + "\n\n" + main
    output


type CompileResult =
    | Good of string
    | Bad of string

let compileWeb input =

    let output = run program input

    match output with
    | Success(declarations, _) -> Good(compile declarations)
    | Failure _ -> Bad(printResult output)
