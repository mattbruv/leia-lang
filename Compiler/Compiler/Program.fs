module Program

open System.Collections.Generic
open System.IO
open Parser
open Compiler

let contents = File.ReadAllText("examples/add.leia")
let result = run program contents

match result with
| Success(literals, _) -> printfn $"%s{compile literals}"
| Failure(s, s1, parserPosition) -> printResult result
