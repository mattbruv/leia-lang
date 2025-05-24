module Parser

open System
open Grammar

type ParserLabel = string
type ParserError = string

type Position = { line: int; column: int }
let initialPos = { line = 0; column = 0 }
let incrCol (pos: Position) = { pos with column = pos.column + 1 }
let incrLine pos = { line = pos.line + 1; column = 0 }

type InputState = { lines: string[]; position: Position }

let currentLine inputState =
    let linePos = inputState.position.line

    if linePos < inputState.lines.Length then
        inputState.lines[linePos]
    else
        "end of file"

type ParserPosition =
    { currentLine: string
      line: int
      column: int }

let parserPositionFromInputState (inputState: InputState) =
    { currentLine = currentLine inputState
      line = inputState.position.line
      column = inputState.position.column }

let fromStr str =
    if String.IsNullOrEmpty(str) then
        { lines = [||]; position = initialPos }
    else
        let separators = [| "\r\n"; "\n" |]
        let lines = str.Split(separators, StringSplitOptions.None)
        { lines = lines; position = initialPos }

let nextChar input =
    let linePos = input.position.line
    let colPos = input.position.column

    if linePos >= input.lines.Length then
        input, None
    else
        let currentLine = currentLine input

        if colPos < currentLine.Length then
            let char = currentLine[colPos]
            let newPos = incrCol input.position
            let newState = { input with position = newPos }
            newState, Some char
        else
            let char = '\n'
            let newPos = incrLine input.position
            let newState = { input with position = newPos }
            newState, Some char

let rec readAllChars input =
    [ let remainingInput, charOpt = nextChar input

      match charOpt with
      | None ->
          // end of input
          ()
      | Some ch ->
          // return first character
          yield ch
          // return the remaining characters
          yield! readAllChars remainingInput ]


type ParseResult<'a> =
    | Success of 'a
    | Failure of ParserLabel * ParserError * ParserPosition

type Parser<'a> =
    { parseFn: InputState -> ParseResult<'a * InputState>
      label: ParserLabel }

let printResult result =
    match result with
    | Success(value, _input) -> printfn $"%A{value}"
    | Failure(label, error, parserPos) ->
        let errorLine = parserPos.currentLine
        let colPos = parserPos.column
        let linePos = parserPos.line
        let failureCaret = sprintf "%*s^%s" colPos "" error
        printfn $"Line: %i{linePos} Col: %i{colPos} Error parsing %s{label}\n%s{errorLine}\n%s{failureCaret}"

let runOnInput parser input = parser.parseFn input

let run parser inputStr = runOnInput parser (fromStr inputStr)

let setLabel parser newLabel =
    let newInnerFn input =
        let result = parser.parseFn input

        match result with
        | Success s -> Success s
        | Failure(_, err, pos) -> Failure(newLabel, err, pos)

    { parseFn = newInnerFn
      label = newLabel }

let (<?>) = setLabel

let getLabel parser = parser.label


let satisfy predicate label =
    let innerFn input =
        let remainingInput, charOpt = nextChar input

        match charOpt with
        | None ->
            let err = "No more input"
            let pos = parserPositionFromInputState input
            Failure(label, err, pos)
        | Some first ->
            if predicate first then
                Success(first, remainingInput)
            else
                let err = $"Unexpected '%c{first}'"
                let pos = parserPositionFromInputState input
                Failure(label, err, pos)

    { parseFn = innerFn; label = label }

let pchar charToMatch =
    let label = $"%c{charToMatch}"
    let predicate ch = (ch = charToMatch)
    satisfy predicate label

let bindP f p =
    let label = "unknown"

    let innerFn input =
        let result1 = runOnInput p input

        match result1 with
        | Failure(label, err, pos) -> Failure(label, err, pos)
        | Success(value1, remainingInput) ->
            let p2 = f value1
            runOnInput p2 remainingInput

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
        let result1 = runOnInput parser1 input

        match result1 with
        | Success _ -> result1
        | Failure _ -> runOnInput parser2 input

    { parseFn = innerFn; label = label }

let mapP f = bindP (f >> returnP)

let (.>>.) = andThen
let (<|>) = orElse
let (<!>) = mapP

/// Alias for mapP
/// in the context of parsing, we’ll often want to put the mapping function after the parser,
/// with the parameters flipped.
/// This makes using map with the pipeline idiom much more convenient
let (|>>) x f = mapP f x

let choice listOfParsers = List.reduce (<|>) listOfParsers

let anyOf listOfChars =
    let label = $"any of %A{listOfChars}"

    listOfChars
    |> List.map pchar // map into a parser
    |> choice // combine them
    <?> label

let parseLowercase = anyOf [ 'a' .. 'z' ]
let parseUppercase = anyOf [ 'A' .. 'Z' ]

let digitChar =
    let predicate = Char.IsDigit
    let label = "digit"
    satisfy predicate label

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
    let label = str
    str |> List.ofSeq |> List.map pchar |> sequence |> mapP charListToStr <?> label

let rec parseZeroOrMore parser input =
    let firstResult = runOnInput parser input

    match firstResult with
    | Failure _ -> ([], input)
    | Success(firstValue, inputAfterFirstParse) ->
        let subsequentValues, remainingInput = parseZeroOrMore parser inputAfterFirstParse
        let values = firstValue :: subsequentValues
        (values, remainingInput)

let many parser =
    let innerFn input = Success(parseZeroOrMore parser input)
    { parseFn = innerFn; label = "many" }

let many1 parser =
    parser >>= (fun head -> many parser >>= (fun tail -> returnP (head :: tail)))

let manyChars cp = many cp |>> charListToStr

let manyChars1 cp = many1 cp |>> charListToStr

let whitespaceChar =
    let predicate = Char.IsWhiteSpace
    let label = "whitespace"
    satisfy predicate label

let whitespace = many whitespaceChar

let whitespace1 = many1 whitespaceChar


let opt p =
    let some = p |>> Some
    let none = returnP None
    some <|> none

let pint =
    let label = "integer"

    let resultToInt (sign, digits) =
        let i = digits |> int

        match sign with
        | Some _ -> Int -i // negate the int
        | None -> Int i

    let digits = manyChars1 digitChar

    opt (pchar '-') .>>. digits |>> resultToInt <?> label

let pfloat =
    let label = "float"

    // helper
    let resultToFloat (((sign, digits1), _point), digits2) =
        let fl = $"%s{digits1}.%s{digits2}" |> float

        match sign with
        | Some _ -> Float -fl
        | None -> Float fl

    let digits = manyChars1 digitChar

    // A float is a sign, digits, point, digits, (ignore exponents for now)
    opt (pchar '-') .>>. digits .>>. pchar '.' .>>. digits |>> resultToFloat
    <?> label

let (.>>) p1 p2 = p1 .>>. p2 |> mapP fst
let (>>.) p1 p2 = p1 .>>. p2 |> mapP snd

let between p1 p2 p3 = p1 >>. p2 .>> p3

let sepBy1 p sep =
    let sepThenP = sep >>. p
    p .>>. many sepThenP |>> fun (p, pList) -> p :: pList

let sepBy p sep = sepBy1 p sep <|> returnP []

let eof =
    let label = "end of input"

    let innerFn input =
        match nextChar input with
        | _, None -> Success((), input) // no more chars = success
        | _, Some c ->
            let pos = parserPositionFromInputState input
            Failure(label, $"Expected end of input but found '{c}'", pos)

    { parseFn = innerFn; label = label }

// Leia grammar parsers

let pbool =
    choice
        [ pstring "true" |>> (fun _ -> Boolean true)
          pstring "false" |>> (fun _ -> Boolean false) ]
    <?> "boolean"

let pidentifier =
    let isAlpha c = Char.IsLetter c
    let isAlphaNum c = Char.IsLetterOrDigit c || c = '_'
    let first = satisfy isAlpha "identifier (start letter)"
    let rest = many (satisfy isAlphaNum "identifier part")

    (first .>>. rest)
    |>> (fun (head, tail) ->
        let chars = head :: tail
        let str = String.Concat chars
        Identifier str)
    <?> "identifier"

let pstringLiteral =
    let escapedQuote = pstring "\\\"" |>> (fun _ -> '"') // optional, if you want to support escaped quotes
    let normalChar = satisfy (fun c -> c <> '"') "non-quote char"

    let stringChars = many (escapedQuote <|> normalChar)

    between (pchar '"') stringChars (pchar '"') |>> String.Concat |>> LString
    <?> "string literal"

let createParserForwardedToRef<'a> () =
    let dummyParser: Parser<'a> =
        let innerFn _ = failwith "unfixed forwarded parser"
        { parseFn = innerFn; label = "unknown" }

    // mutable pointer to placeholder Parser
    let parserRef = ref dummyParser

    // wrapper parser
    let innerFn input = runOnInput parserRef.Value input
    let wrapperParser = { parseFn = innerFn; label = "unknown" }

    wrapperParser, parserRef


let pexpression, pexpressionRef = createParserForwardedToRef<Expression> ()

let pgrouping =
    between ((pchar '(') .>> whitespace) pexpression (whitespace >>. (pchar ')'))

// A primary expression is either a literal value or a grouping
let pprimary: Parser<Expression> =
    let literalParsers =
        [ pstringLiteral; pfloat; pint; pbool; pidentifier ]
        |> List.map (fun p -> p |>> Literal)

    choice (pgrouping :: literalParsers)

let pfactor: Parser<Expression> =
    let operator = (pchar '*') <|> (pchar '/')

    let opAndPrimary = whitespace >>. operator .>> whitespace .>>. pprimary

    pprimary .>>. many opAndPrimary
    |>> fun (first, rest) ->
        // fold the list into a binary operation chain
        rest
        |> List.fold
            (fun acc (op, next) ->
                match op with
                | '*' -> BinaryOp(Multiply, acc, next)
                | '/' -> BinaryOp(Divide, acc, next)
                | _ -> failwith $"Unexpected operator: {op}")
            first

let pterm: Parser<Expression> =
    let operator = (pchar '+') <|> (pchar '-')
    let opAndFactor = whitespace >>. operator .>> whitespace .>>. pfactor

    pfactor .>>. many opAndFactor
    |>> fun (first, rest) ->
        // fold the list into a binary operation chain
        rest
        |> List.fold
            (fun acc (op, next) ->
                match op with
                | '+' -> BinaryOp(Add, acc, next)
                | '-' -> BinaryOp(Subtract, acc, next)
                | _ -> failwith $"Unexpected operator: {op}")
            first

let pstatement: Parser<Statement> =
    let printStatement = (pstring "print") >>. (whitespace >>. pexpression) |>> Print

    printStatement


pexpressionRef.Value <- choice [ pterm ]

let program =
    many whitespaceChar >>. sepBy1 pstatement whitespace1
    .>> many whitespaceChar
    .>> eof
