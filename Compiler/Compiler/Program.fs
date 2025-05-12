module Foo

let foo = false

let x =
    match foo with
    | true -> "im true"
    | _ -> "todo"

printfn $"%s{x}"
