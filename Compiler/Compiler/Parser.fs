module Parser

open System

type ParseResult<'a> =
    | Success of 'a
    | Failure of string

type Parser<'T> = Parser of (string -> ParseResult<'T * string>)

let run parser input =
    let (Parser innerFn) = parser
    innerFn input

let pchar charToMatch =
    let innerFn str =
        if String.IsNullOrEmpty(str) then
            Failure "No more input"
        else
            let first = str[0]

            if first = charToMatch then
                let remaining = str[1..]
                Success(charToMatch, remaining)
            else
                let msg = $"Expecting '%c{charToMatch}'. Got '%c{first}'"
                Failure msg

    Parser innerFn
