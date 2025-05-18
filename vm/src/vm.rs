use crate::instruction::{ConstantValue, Opcode, Program};

#[derive(Debug)]
pub struct VM {
    pc: usize,
    program: Program,
}

impl VM {
    pub fn new(program: Program) -> VM {
        VM { pc: 0, program }
    }

    pub fn run(&mut self) -> () {
        loop {
            if self.pc >= self.program.code.len() {
                break;
            }

            self.pc += 1;
        }
    }
}
