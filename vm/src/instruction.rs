use std::fmt::Display;

#[derive(Debug)]
pub struct Program {
    pub code: Vec<Opcode>,
    pub constants: Vec<ConstantValue>,
}

#[derive(Debug, PartialEq)]
pub struct ConstantIndex(pub u32);

#[derive(Debug, PartialEq)]
pub enum Opcode {
    Push(ConstantIndex),
    Jump(usize),
    Add,
    Print,
    Halt,
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
}

#[derive(Debug, PartialEq)]
pub enum LeiaValue {
    Int(i32),
    Float(f32),
    Str(String),
}

impl Display for LeiaValue {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        match self {
            LeiaValue::Int(x) => write!(f, "{x}"),
            LeiaValue::Float(x) => write!(f, "{x}"),
            LeiaValue::Str(x) => write!(f, "\"{x}\""),
        }
    }
}
