module Program

open System.Xml
open Parser

let inputABC = "ABC"
let parseA = pchar 'A'
let parseB = pchar 'B'


let parseAThenB = parseA .>>. parseB
let parseAOrB = parseA <|> parseB

let x = run parseAThenB inputABC
let y = run parseAOrB inputABC

let aAndThenBorC = parseA .>>. (parseB <|> pchar 'C')

let z = run aAndThenBorC inputABC

printfn $"{x}"
printfn $"{y}"
printfn $"{z}"
