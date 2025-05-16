module Program

open System.IO
open Parser

let program =
    many whitespaceChar >>. sepBy1 pexpression whitespace1
    .>> many whitespaceChar
    .>> eof

let contents = File.ReadAllText("examples/syntax.leia")

run program contents |> printResult
