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
    Sub,
    Mul,
    Div,
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

macro_rules! impl_arith_op {
    ($name:ident, $symbol:tt, $err:expr) => {
        pub fn $name(self, other: LeiaValue) -> LeiaValue {
            match (self, other) {
                (LeiaValue::Int(a), LeiaValue::Int(b)) => LeiaValue::Int(a $symbol b),
                (LeiaValue::Float(a), LeiaValue::Float(b)) => LeiaValue::Float(a $symbol b),
                _ => panic!($err),
            }
        }
    };
}

impl LeiaValue {
    impl_arith_op!(add, +, "Invalid types for addition");
    impl_arith_op!(sub, -, "Invalid types for subtraction");
    impl_arith_op!(mul, *, "Invalid types for multiplication");
    impl_arith_op!(div, /, "Invalid types for division");

    // You can still write specialized ones by hand, like string concatenation
    pub fn add_string(self, other: LeiaValue) -> LeiaValue {
        match (self, other) {
            (LeiaValue::Str(a), LeiaValue::Str(b)) => LeiaValue::Str(a + &b),
            _ => panic!("Invalid types for string addition"),
        }
    }
}
