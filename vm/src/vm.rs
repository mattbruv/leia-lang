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
                    let b = self.stack.pop().unwrap();
                    let a = self.stack.pop().unwrap();
                    self.stack.push(a.add(b));
                }
                Opcode::Sub => {
                    let b = self.stack.pop().unwrap();
                    let a = self.stack.pop().unwrap();
                    self.stack.push(a.sub(b));
                }
                Opcode::Mul => {
                    let b = self.stack.pop().unwrap();
                    let a = self.stack.pop().unwrap();
                    self.stack.push(a.mul(b));
                }
                Opcode::Div => {
                    let b = self.stack.pop().unwrap();
                    let a = self.stack.pop().unwrap();
                    self.stack.push(a.div(b));
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
