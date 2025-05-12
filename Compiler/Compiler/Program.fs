module Program

open Parser
let inputABC = "ABC"
let parseA = pchar 'A'
let x = run parseA inputABC
printfn $"{x}"
