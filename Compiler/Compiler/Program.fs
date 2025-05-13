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

let print x = printfn $"%A{x}"


print (run parseLowercase "aBC")
print (run parseLowercase "ABC")
print (run parseLowercase "abc")
print (run parseDigit "1ABC")
print (run parseDigit "9ABC")
print (run parseDigit "|ABC")

let parseThreeDigitsAsStr =
    let tupleParser = parseDigit .>>. parseDigit .>>. parseDigit
    let transformTuple ((c1, c2), c3) = System.String [| c1; c2; c3 |]
    mapP transformTuple tupleParser

let parseThreeDigitsAsInt = parseThreeDigitsAsStr |>> int

print (run parseThreeDigitsAsInt "123A")

let parsers = [ pchar 'A'; pchar 'B'; pchar 'C' ]
let combined = sequence parsers
print (run combined "ABCD")

let parseABC = pstring "ABC"
print (run parseABC "ABCDE")
print (run parseABC "A|CDE")
print (run parseABC "AB|DE")
