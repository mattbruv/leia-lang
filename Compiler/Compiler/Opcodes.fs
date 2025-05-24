module Opcodes

type Opcode =
    //
    | PushConstant of int

    | Call of string
    | Return

    | Jump of string
    | JumpIfZero of string
    | JumpIfNotZero of string

    | Equals
    | GreaterThan

    | Add
    | Sub
    | Mul
    | Div
    | Mod

    | Print
    | Halt
