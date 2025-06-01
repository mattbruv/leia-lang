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

type Ident = Ident of string

module Ident =
    let value (Ident s) = s

type Literal =
    | Int of int
    | Float of float
    | LString of string
    | Boolean of bool
    | Identifier of Ident

type FunctionCall =
    { name: Ident
      arguments: Expression list }

and Expression =
    | Literal of Literal
    | BinaryOp of BinaryOp * Expression * Expression
    | Assignment of Ident * Expression
    | Call of FunctionCall

type Function =
    { name: Ident
      parameters: Ident list option
      body: Declaration list }

and Declaration =
    | Function of Function
    | Statement of Statement

and Statement =
    //
    | Print of Expression
    | If of Expression * Statement * Statement option
    | Block of Declaration list
    | Expr of Expression
