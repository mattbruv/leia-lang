module Program

open Parser

let print x = printfn $"%A{x}"

print (run pint "123C")
print (run pint "-123C")

let digit = anyOf [ '0' .. '9' ]
let digitThenSemicolon = digit .>> opt (pchar ';')
print (run digitThenSemicolon "1;")
print (run digitThenSemicolon "1")

let ab = pstring "AB"
let cd = pstring "CD"
let ab_cd = (ab .>> whitespace) .>>. cd

print (run ab_cd "AB   \t   CDhi")

let pdoublequote = pchar '"'
let quotedInteger = between pdoublequote pint pdoublequote

print (run quotedInteger "\"1234\"") // Success (1234, "")
print (run quotedInteger "1234") // Failure "Expecting '"'. Got '1'"
