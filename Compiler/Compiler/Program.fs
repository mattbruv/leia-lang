module Program

open Parser

let print x = printfn $"%A{x}"

print (run pint "123C")
print (run pint "-123C")
