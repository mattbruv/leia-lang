module Parser

open System

type ParserLabel = string
type ParserError = string

type ParseResult<'a> =
    | Success of 'a
    | Failure of ParserLabel * ParserError


type Parser<'a> =
    { parseFn: (string -> ParseResult<'a * string>)
      label: ParserLabel }

let printResult result =
    match result with
    | Success(value, _input) -> printfn $"%A{value}"
    | Failure(label, error) -> printfn $"Error parsing %s{label}\n%s{error}"

// type Parser<'T> = Parser of (string -> ParseResult<'T * string>)

let run (parser: Parser<_>) input = parser.parseFn input

let setLabel parser newLabel =
    let newInnerFn input =
        let result = parser.parseFn input

        match result with
        | Success s -> Success s
        | Failure(_, err) -> Failure(newLabel, err)

    { parseFn = newInnerFn
      label = newLabel }

let (<?>) = setLabel

let getLabel parser = parser.label

let pchar charToMatch =
    let innerFn str =
        if String.IsNullOrEmpty(str) then
            Failure("char", "No more input")
        else
            let first = str[0]

            if first = charToMatch then
                let remaining = str[1..]
                Success(charToMatch, remaining)
            else
                let msg = $"Unexpected '%c{first}'"
                Failure("char", msg)

    { parseFn = innerFn; label = "char" }

let bindP f p =
    let label = "unknown"

    let innerFn input =
        let result1 = run p input

        match result1 with
        | Failure(label, err) -> Failure(label, err)
        | Success(value1, remainingInput) ->
            let p2 = f value1
            run p2 remainingInput

    { parseFn = innerFn; label = label }

let (>>=) p f = bindP f p

let returnP x =
    let innerFn input = Success(x, input)
    { parseFn = innerFn; label = "returnP" }


let andThen p1 p2 =
    let label = $"%s{getLabel p1} andThen %s{getLabel p2}"

    p1 >>= (fun p1Result -> p2 >>= (fun p2Result -> returnP (p1Result, p2Result)))
    <?> label

let orElse parser1 parser2 =
    let label = $"%s{getLabel parser1} orElse %s{getLabel parser2}"

    let innerFn input =
        let result1 = run parser1 input

        match result1 with
        | Success _ -> result1
        | Failure _ -> run parser2 input

    { parseFn = innerFn; label = label }

let mapP f = bindP (f >> returnP)

let (.>>.) = andThen
let (<|>) = orElse
let (<!>) = mapP

// in the context of parsing, we’ll often want to put the mapping function after the parser,
// with the parameters flipped.
// This makes using map with the pipeline idiom much more convenient
let (|>>) x f = mapP f x

let choice listOfParsers = List.reduce (<|>) listOfParsers

let anyOf listOfChars =
    let label = $"any of %A{listOfChars}"

    listOfChars
    |> List.map pchar // map into a parser
    |> choice // combine them
    <?> label

let parseLowercase = anyOf [ 'a' .. 'z' ]
let parseDigit = anyOf [ '0' .. '9' ]

let applyP fP xP =
    fP >>= (fun f -> xP >>= (fun x -> returnP (f x)))

let (<*>) = applyP

let lift2 f xP yP = returnP f <*> xP <*> yP

let addP = lift2 (+)

let startsWith (str: string) (prefix: string) = str.StartsWith(prefix)

let startsWithP = lift2 startsWith

let rec sequence parserList =
    let cons head tail = head :: tail
    let consP = lift2 cons

    match parserList with
    | [] -> returnP []
    | head :: tail -> consP head (sequence tail)

let charListToStr charList = charList |> List.toArray |> String

let pstring str =
    str |> List.ofSeq |> List.map pchar |> sequence |> mapP charListToStr

let rec parseZeroOrMore parser input =
    let firstResult = run parser input

    match firstResult with
    | Failure _ -> ([], input)
    | Success(firstValue, inputAfterFirstParse) ->
        let (subsequentValues, remainingInput) = parseZeroOrMore parser inputAfterFirstParse
        let values = firstValue :: subsequentValues
        (values, remainingInput)

let many parser =
    let innerFn input = Success(parseZeroOrMore parser input)
    { parseFn = innerFn; label = "many" }

let whitespaceChar = anyOf [ ' '; '\t'; '\n' ]
let whitespace = many whitespaceChar


let many1 parser =
    parser >>= (fun head -> many parser >>= (fun tail -> returnP (head :: tail)))

let opt p =
    let some = p |>> Some
    let none = returnP None
    some <|> none

let pint =
    let resultToInt (sign, charList) =
        let i = charList |> List.toArray |> String |> int

        match sign with
        | Some ch -> -i
        | None -> i

    let digit = anyOf [ '0' .. '9' ]
    let digits = many1 digit

    opt (pchar '-') .>>. digits |>> resultToInt

let (.>>) p1 p2 = p1 .>>. p2 |> mapP fst
let (>>.) p1 p2 = p1 .>>. p2 |> mapP snd

let between p1 p2 p3 = p1 >>. p2 .>> p3

let sepBy1 p sep =
    let sepThenP = sep >>. p
    p .>>. many sepThenP |>> fun (p, pList) -> p :: pList

let sepBy p sep = sepBy1 p sep <|> returnP []
