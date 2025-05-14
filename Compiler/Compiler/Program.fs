module Program

open Parser

let print x = printfn $"%A{x}"

let parseAB = pchar 'A' .>>. pchar 'B' <?> "AB"
run parseAB "A|C" |> printResult
