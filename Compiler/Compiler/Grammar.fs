module Grammar

open System.Linq.Expressions

type BinaryOp =
    | Multiply
    | Divide
    | Add
    | Subtract

type Literal =
    | Int of int
    | Float of float
    | LString of string
    | Boolean of bool
    | Identifier of string

type Expression =
    | Literal of Literal
    | BinaryOp of BinaryOp * Expression * Expression

type Statement =
    //
    | Print of Expression


// a parser of a literal should be one of pint, pstring, or pbool
