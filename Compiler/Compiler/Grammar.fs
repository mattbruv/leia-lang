module Grammar

type Literal =
    | Int of int
    | Float of float
    | LString of string
    | Boolean of bool

type Expression =
    //
    | Literal of Literal

// a parser of a literal should be one of pint, pstring, or pbool
