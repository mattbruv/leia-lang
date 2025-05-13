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

let orElse parser1 parser2 =
    let innerFn input =
        let result1 = run parser1 input

        match result1 with
        | Success _ -> result1
        | Failure _ -> run parser2 input

    Parser innerFn

let mapP f parser =
    let innerFn input =
        let result = run parser input

        match result with
        | Success(value, remaining) ->
            let newValue = f value
            Success(newValue, remaining)
        | Failure err -> Failure err

    Parser innerFn

let (.>>.) = andThen
let (<|>) = orElse
let (<!>) = mapP

// in the context of parsing, we’ll often want to put the mapping function after the parser,
// with the parameters flipped.
// This makes using map with the pipeline idiom much more convenient
let (|>>) x f = mapP f x

let choice listOfParsers = List.reduce (<|>) listOfParsers

let anyOf listOfChars =
    listOfChars
    |> List.map pchar // map into a parser
    |> choice // combine them

let parseLowercase = anyOf [ 'a' .. 'z' ]
let parseDigit = anyOf [ '0' .. '9' ]
