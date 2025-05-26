module Opcodes

type Opcode =
    //
    | PushConstant of int

    | Call of string
    | Return

    | Jump of string
    | JumpIfZero of string
    | JumpIfNotZero of string

    | LoadLocal of int
    | StoreLocal of int

    | Eq
    | NotEq
    | Gt
    | Gte
    | Lt
    | Lte

    | Add
    | Sub
    | Mul
    | Div
    | Mod

    | Print
    | Halt
