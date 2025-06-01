use std::fmt::Display;

#[derive(Debug)]
pub struct Program {
    pub entry: usize,
    pub code: Vec<Opcode>,
    pub constants: Vec<ConstantValue>,
}

#[derive(Debug, PartialEq, Clone)]
pub struct ConstantIndex(pub u32);

#[derive(Debug, PartialEq, Clone)]
pub enum Opcode {
    Push(ConstantIndex), // push a constant value

    Pop, // pop the top value off the stack and dispose of it

    Call(usize), // call a function
    Return,      // Return from a function

    Jump(usize),          // unconditional jump
    JumpIfZero(usize),    // jump if popped value == 0
    JumpIfNotZero(usize), // jump if popped value != 0

    LoadLocal(usize),  // push from local variable slot
    StoreLocal(usize), // pop into local variable slot
    Increment(usize),

    Equals,
    NotEqual,
    GreaterThan,
    GreaterThanEqual,
    LessThan,
    LessThanEqual,

    Add,
    Subtract,
    Multiply,
    Divide,
    Modulo,

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

#[derive(Debug, PartialEq, Clone)]
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
            LeiaValue::Str(x) => write!(f, "{x}"),
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

macro_rules! impl_cmp_op {
    ($name:ident, $symbol:tt, $err:expr) => {
        pub fn $name(self, other: LeiaValue) -> LeiaValue {
            match (self, other) {
                (LeiaValue::Int(a), LeiaValue::Int(b)) => {
                    LeiaValue::Int((a $symbol b) as i32)
                }
                (LeiaValue::Float(a), LeiaValue::Float(b)) => {
                    LeiaValue::Int((a $symbol b) as i32)
                }
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
    impl_arith_op!(modulo, %, "Invalid types for division");

    // You can still write specialized ones by hand, like string concatenation
    pub fn add_string(self, other: LeiaValue) -> LeiaValue {
        match (self, other) {
            (LeiaValue::Str(a), LeiaValue::Str(b)) => LeiaValue::Str(a + &b),
            _ => panic!("Invalid types for string addition"),
        }
    }

    impl_cmp_op!(gt, >, "Invalid types for greater-than comparison");
    impl_cmp_op!(lt, <, "Invalid types for less-than comparison");
    impl_cmp_op!(lte, <=, "Invalid types for less-than-equal comparison");
    impl_cmp_op!(gte, >=, "Invalid types for greater-than-equal comparison");
    impl_cmp_op!(eq, ==, "Invalid types for equality comparison");
    impl_cmp_op!(neq, !=, "Invalid types for equality comparison");
    impl_cmp_op!(ne, !=, "Invalid types for inequality comparison");
}
