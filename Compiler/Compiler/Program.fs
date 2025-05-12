module Program

open Parser


let inputABC = "ABC"
let x = pchar ('A', inputABC)

printfn $"{x}"
