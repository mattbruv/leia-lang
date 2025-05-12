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

let andThen parser1 parser2 =
    let innerFn input =
        let result1 = run parser1 input

        match result1 with
        | Failure err -> Failure err
        | Success(value1, remaining1) ->
            let result2 = run parser2 remaining1

            match result2 with
            | Failure err -> Failure err
            | Success(value2, remaining2) ->
                let newValue = (value1, value2)
                Success(newValue, remaining2)

    Parser innerFn

let (.>>.) = andThen
