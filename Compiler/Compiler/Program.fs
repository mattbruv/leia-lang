module Program

open Parser

let program =
    many whitespaceChar >>. sepBy1 pexpression whitespace1
    .>> many whitespaceChar
    .>> eof


run program "10.0 false true 1 2 3 1234 foobarbaz function print"
//
|> printResult
