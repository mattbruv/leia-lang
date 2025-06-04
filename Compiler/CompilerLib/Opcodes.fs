module Opcodes

type Label = Label of string

module Label =
    let value (Label s) = s

type Opcode =
    //
    | PushConstant of int
    | Pop

    | Call of Label
    | Return

    | Jump of Label
    | JumpIfZero of Label
    | JumpIfNotZero of Label

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
