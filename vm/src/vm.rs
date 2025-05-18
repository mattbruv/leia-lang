use crate::instruction::{ConstantValue, LeiaValue, Opcode, Program};

#[derive(Debug)]
pub struct VM {
    pc: usize,
    program: Program,
    stack: Vec<LeiaValue>,
}

impl VM {
    pub fn new(program: Program) -> VM {
        VM {
            pc: 0,
            program,
            stack: vec![],
        }
    }

    pub fn run(&mut self) -> () {
        loop {
            //println!("{:?}", self.stack);
            if self.pc >= self.program.code.len() {
                break;
            }

            match &self.program.code[self.pc] {
                Opcode::Push(constant_index) => {
                    let constant = &self.program.constants[constant_index.0 as usize];
                    self.stack.push(match constant {
                        ConstantValue::Int(x) => LeiaValue::Int(*x),
                        ConstantValue::Float(x) => LeiaValue::Float(*x),
                        ConstantValue::Str(x) => LeiaValue::Str(x.clone()),
                    });
                }
                Opcode::Jump(addr) => self.pc = *addr,
                Opcode::Add => {
                    let a = self.stack.pop().unwrap();
                    let b = self.stack.pop().unwrap();

                    match (a, b) {
                        (LeiaValue::Int(y), LeiaValue::Int(x)) => {
                            self.stack.push(LeiaValue::Int(x + y));
                        }
                        (LeiaValue::Float(y), LeiaValue::Float(x)) => {
                            self.stack.push(LeiaValue::Float(x + y));
                        }
                        (LeiaValue::Str(y), LeiaValue::Str(x)) => {
                            self.stack.push(LeiaValue::Str(x + &y));
                        }
                        _ => panic!("Invalid type addition"),
                    }
                }
                Opcode::Print => {
                    let val = self.stack.pop().unwrap();
                    println!("{}", val);
                }
                Opcode::Halt => {
                    break;
                }
            }

            self.pc += 1;
        }
    }
}
