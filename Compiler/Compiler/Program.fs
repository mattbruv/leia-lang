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

let comma = pchar ','

let zeroOrMoreDigitList = sepBy digit comma
let oneOrMoreDigitList = sepBy1 digit comma

print (run oneOrMoreDigitList "1;") // Success (['1'], ";")
print (run oneOrMoreDigitList "1,2;") // Success (['1'; '2'], ";")
print (run oneOrMoreDigitList "1,2,3;") // Success (['1'; '2'; '3'], ";")
print (run oneOrMoreDigitList "Z;") // Failure "Expecting '9'. Got 'Z'"

print (run zeroOrMoreDigitList "1;") // Success (['1'], ";")
print (run zeroOrMoreDigitList "1,2;") // Success (['1'; '2'], ";")
print (run zeroOrMoreDigitList "1,2,3;") // Success (['1'; '2'; '3'], ";")
print (run zeroOrMoreDigitList "Z;") // Success ([], "Z;")
