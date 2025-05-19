use crate::instruction::{ConstantValue, LeiaValue, Opcode, Program};

#[derive(Debug)]
pub struct VM {
    pc: usize,
    program: Program,
    stack: Vec<LeiaValue>,
    locals: Vec<LeiaValue>,
}

impl VM {
    pub fn new(program: Program) -> VM {
        VM {
            pc: 0,
            program,
            stack: vec![],
            locals: vec![],
        }
    }

    pub fn run(&mut self) -> () {
        loop {
            //println!("{:?}", self.stack);
            if self.pc >= self.program.code.len() {
                break;
            }

            let code = &self.program.code[self.pc];
            //println!("{}: {:?} {:?}", self.pc, code, self.locals);

            match code {
                Opcode::Push(constant_index) => {
                    let constant = &self.program.constants[constant_index.0 as usize];
                    self.stack.push(match constant {
                        ConstantValue::Int(x) => LeiaValue::Int(*x),
                        ConstantValue::Float(x) => LeiaValue::Float(*x),
                        ConstantValue::Str(x) => LeiaValue::Str(x.clone()),
                    });
                }
                Opcode::Jump(addr) => self.pc = *addr - 1, // adds one back right after this to go to the correct spot
                Opcode::Add => {
                    let b = self.stack.pop().unwrap();
                    let a = self.stack.pop().unwrap();
                    self.stack.push(a.add(b));
                }
                Opcode::Subtract => {
                    let b = self.stack.pop().unwrap();
                    let a = self.stack.pop().unwrap();
                    self.stack.push(a.sub(b));
                }
                Opcode::Multiply => {
                    let b = self.stack.pop().unwrap();
                    let a = self.stack.pop().unwrap();
                    self.stack.push(a.mul(b));
                }
                Opcode::Divide => {
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
                Opcode::JumpIfZero(addr) => {
                    let val = self.stack.pop().unwrap();
                    //println!("JUMP? {:?}", val);
                    match val {
                        LeiaValue::Int(x) => {
                            if x == 0 {
                                self.pc = *addr - 1; // subtracting one because we add it right back after this
                            }
                        }
                        _ => panic!("Invalid jp if zero value!"),
                    }
                }
                Opcode::LoadLocal(idx) => {
                    let val = self
                        .locals
                        .get(*idx)
                        .expect("Local variable index out of bounds");
                    self.stack.push(val.clone());
                }
                Opcode::StoreLocal(idx) => {
                    let val = self.stack.pop().expect("Stack underflow on StoreLocal");

                    if *idx == self.locals.len() {
                        // Append the new local since it's exactly the next index
                        self.locals.push(val);
                    } else if *idx < self.locals.len() {
                        // Overwrite existing local
                        self.locals[*idx] = val;
                    } else {
                        panic!("Local variable index out of bounds: {}", idx);
                    }
                }

                Opcode::Equals => todo!(),
            }

            self.pc += 1;
        }
    }
}
