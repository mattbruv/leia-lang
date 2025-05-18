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
