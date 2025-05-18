/**
    add/sub/mul/div/rem
    cmp
    je/jne/jg/jge/jl/jle
    logical and/or
    bitwise and/or
    not/xor/neg
    left/right shift
    load/store
    alloc/free
    ret
    special instructions to handle arrays/structs/strings/tuples/vectors etc
*/

#[derive(Debug, PartialEq)]
pub struct ConstantIndex(u16);

#[derive(Debug, PartialEq)]
pub enum Opcode {
    LoadConstant(ConstantIndex),
    Halt,
    Illegal,
}

#[derive(Debug, PartialEq)]
pub struct Constant {
    index: ConstantIndex,
    value: ConstantValue,
}

#[derive(Debug, PartialEq)]
pub enum ConstantValue {
    Int(i32),
    Float(f32),
    Str(String),
    Bool(bool),
}

impl From<u8> for Opcode {
    fn from(value: u8) -> Self {
        match value {
            0 => return Opcode::Halt,
            _ => return Opcode::Illegal,
        }
    }
}
