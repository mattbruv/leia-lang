module Program

open Parser
let inputABC = "ABC"
let parseA = pchar 'A'
let parseB = pchar 'B'


let parseAThenB = parseA .>>. parseB

let x = run parseAThenB inputABC
printfn $"{x}"
