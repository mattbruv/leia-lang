module Parser

open System


type ParseResult<'a> =
    | Success of 'a
    | Failure of string

let pchar (charToMatch, str) =
    if String.IsNullOrEmpty(str) then
        Failure "No more input"
    else
        let first = str[0]
        if first = charToMatch then
            let remaining = str[1..]
            let msg = sprintf "Found %c" charToMatch
            Success (charToMatch, remaining)
        else
            let msg = sprintf "Expecting '%c'. Got '%c'" charToMatch first
            Failure msg
        