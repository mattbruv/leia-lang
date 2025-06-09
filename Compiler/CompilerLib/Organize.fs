module Organize

open Grammar

// let rec assignmentsInExpr =
//     function
//     | Assignment(id, expr) -> Assignment(id, expr) :: assignmentsInExpr expr
//     | BinaryOp(_, l, r) -> assignmentsInExpr l @ assignmentsInExpr r
//     | Call { arguments = args } -> List.collect assignmentsInExpr args
//     | Literal _ -> []
//
// let rec assignmentsInStmt =
//     function
//     | Print e
//     | ExprStatement e -> assignmentsInExpr e
//     | If(cond, t, f) ->
//         let FUCK =
//             match f with
//             | Some value -> assignmentsInStmt value
//             | None -> []
//
//         assignmentsInExpr cond @ assignmentsInStmt t @ FUCK
//     | Block decls -> List.collect assignmentsInDecl decls
//
// and assignmentsInDecl =
//     function
//     | FunctionDeclaration _ -> []
//     | Statement s -> assignmentsInStmt s
//
// let extractAssignments decls = List.collect assignmentsInDecl decls
//
let organize (program: Declaration list) =
    // Check for main function at top level.
    // If one doesn't exist, bundle all top level statements into it.

    let maybeMain =
        program
        |> List.tryPick (function
            | FunctionDeclaration fn when (Ident.value fn.name = "main") -> Some(fn)
            | _ -> None)

    let topLevelDeclarations =
        program
        |> List.choose (function
            | FunctionDeclaration fn when Ident.value fn.name <> "main" -> Some(FunctionDeclaration fn)
            | _ -> None)

    let topLevelStatements =
        program
        |> List.choose (function
            | Statement s -> Some s
            | _ -> None)

    let output: Declaration list =
        match maybeMain with
        | Some main ->
            // If we have defined a main function but still have top level statements, throw an error
            if List.length topLevelStatements > 0 then
                failwith "main() is explicitly defined but there are top level statements outside of main."

            [ FunctionDeclaration main ] @ topLevelDeclarations
        | None ->
            // If the user didn't define main, let's create our own.
            let stmts = topLevelStatements |> List.map Statement

            let main =
                FunctionDeclaration
                    { name = Ident "main"
                      parameters = None
                      body = stmts }

            [ main ] @ topLevelDeclarations

    output
