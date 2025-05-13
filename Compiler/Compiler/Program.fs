module Program

open Parser

let print x = printfn $"%A{x}"


let parseDigit_WithLabel = anyOf [ '0' .. '9' ] <?> "digit"

run parseDigit_WithLabel "|ABC" |> printResult
