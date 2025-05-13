module Program

open Parser

let print x = printfn $"%A{x}"


let x = fromStr "a\nb" |> readAllChars

print x
