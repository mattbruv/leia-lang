module Grammar

type BinaryOp =
    | Multiply
    | Divide
    | Modulo
    | Add
    | Subtract

    | GreaterThan
    | GreaterThanEqual
    | LessThan
    | LessThanEqual

    | NotEqual
    | Equal

    | And
    | Or

type Literal =
    | Int of int
    | Float of float
    | LString of string
    | Boolean of bool
    | Identifier of string

type Expression =
    | Literal of Literal
    | BinaryOp of BinaryOp * Expression * Expression
    | Assignment of string * Expression

type Statement =
    //
    | Print of Expression
    | Expr of Expression


// a parser of a literal should be one of pint, pstring, or pbool
