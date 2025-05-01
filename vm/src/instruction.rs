#[derive(Debug, PartialEq)]
pub struct Instruction {
    opcode: Opcode,
}

#[derive(Debug, PartialEq)]
pub enum Opcode {
    Halt,
    Illegal,
}

impl Instruction {
    pub fn new(opcode: Opcode) -> Instruction {
        Instruction { opcode }
    }
}

impl From<u8> for Opcode {
    fn from(value: u8) -> Self {
        match value {
            0 => return Opcode::Halt,
            _ => return Opcode::Illegal,
        }
    }
}
