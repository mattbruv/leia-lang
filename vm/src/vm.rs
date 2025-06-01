use crate::instruction::{ConstantValue, LeiaValue, Opcode, Program};

type OutputHandler = Box<dyn FnMut(&LeiaValue)>;

pub struct VM {
    pc: usize,
    program: Program,
    stack: Vec<LeiaValue>,
    call_stack: Vec<StackFrame>,
    output_handler: Option<OutputHandler>,
}

#[derive(Debug, Clone)]
pub struct StackFrame {
    return_address: usize,
    locals: Vec<LeiaValue>,
}

impl VM {
    pub fn new(program: Program) -> VM {
        VM {
            pc: program.entry,
            program,
            stack: vec![],
            call_stack: vec![StackFrame {
                locals: vec![],
                return_address: 0,
            }],
            output_handler: None,
        }
    }

    fn locals(&self) -> &Vec<LeiaValue> {
        &self.call_stack.last().unwrap().locals
    }

    fn locals_mut(&mut self) -> &mut Vec<LeiaValue> {
        &mut self.call_stack.last_mut().unwrap().locals
    }

    pub fn run(&mut self) -> () {
        //println!("PC START: {}", self.pc);
        loop {
            //println!("{:?}", self.stack);
            if self.pc >= self.program.code.len() {
                break;
            }

            // Temp hack: clone the opcode for now
            // Having some borrow checker issues having it as an immutable ref to self
            // and then using a mutable self ref later.
            // TODO: maybe write a separate function which passes the touched properties as args to avoid this?
            let code = self.program.code[self.pc].clone();
            /*
            println!(
                "{}: code {:?} stack: {:?} locals: {:?}",
                self.pc,
                code,
                self.stack,
                self.locals()
            );
            */

            match code {
                Opcode::Pop => {
                    self.stack
                        .pop()
                        .expect("Attempted to pop a value where none exists!");
                }
                Opcode::Push(constant_index) => {
                    //println!("{:?}", self.program.constants);
                    //println!("push const: {:?}", constant_index);
                    let constant = &self.program.constants[constant_index.0 as usize];
                    self.stack.push(match constant {
                        ConstantValue::Int(x) => LeiaValue::Int(*x),
                        ConstantValue::Float(x) => LeiaValue::Float(*x),
                        ConstantValue::Str(x) => LeiaValue::Str(x.clone()),
                    });
                }
                Opcode::Jump(addr) => {
                    self.pc = addr;
                    continue; // don't increment pc
                }
                Opcode::Increment(idx) => {
                    if let Some(local) = self.locals_mut().get_mut(idx) {
                        match local {
                            LeiaValue::Int(n) => *n += 1,
                            _ => panic!("Cannot increment non-int local"),
                        }
                    } else {
                        panic!("Local variable index out of bounds: {}", idx);
                    }
                }
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
                Opcode::Modulo => {
                    let b = self.stack.pop().unwrap();
                    let a = self.stack.pop().unwrap();
                    self.stack.push(a.modulo(b));
                }
                Opcode::Print => {
                    let val = self.stack.pop().unwrap();
                    if let Some(handler) = self.output_handler.as_mut() {
                        handler(&val);
                    } else {
                        println!("{}", val);
                    }
                }
                Opcode::Halt => {
                    if (self.stack.len()) > 0 {
                        panic!("Stack length is not zero!: {}", self.stack.len());
                    }
                    break;
                }
                Opcode::JumpIfZero(addr) => {
                    // we don't pop the value off the stack here when comparing.
                    // This is to support logical operators where we don't want to pop the condition
                    // See "jumping back and forth" chapter in Crafting interpreters
                    let val = self.stack.last().unwrap(); // .pop().unwrap();
                    //println!("JUMP? {:?}", val);
                    match val {
                        LeiaValue::Int(x) => {
                            if *x == 0 {
                                self.pc = addr; // - 1; 
                                continue; // don't increment PC
                            }
                        }
                        _ => panic!("Invalid jp if zero value!"),
                    }
                }
                Opcode::JumpIfNotZero(addr) => {
                    let val = self.stack.last().unwrap(); //.pop().unwrap();
                    //println!("JUMP? {:?}", val);
                    match val {
                        LeiaValue::Int(x) => {
                            if *x != 0 {
                                self.pc = addr;
                                continue; // don't increment PC
                            }
                        }
                        _ => panic!("Invalid jp if zero value!"),
                    }
                }
                Opcode::LoadLocal(idx) => {
                    let val = self
                        .locals()
                        .get(idx)
                        .expect("Local variable index out of bounds")
                        .clone();
                    self.stack.push(val);
                }
                Opcode::StoreLocal(idx) => {
                    let val = self
                        .stack
                        .pop()
                        .expect(format!("Stack underflow on StoreLocal index {}", idx).as_str());

                    if idx == self.locals_mut().len() {
                        // Append the new local since it's exactly the next index
                        self.locals_mut().push(val);
                    } else if idx < self.locals_mut().len() {
                        // Overwrite existing local
                        self.locals_mut()[idx] = val;
                    } else {
                        panic!("Local variable index out of bounds: {}", idx);
                    }
                }
                Opcode::Equals => {
                    let b = self.stack.pop().unwrap();
                    let a = self.stack.pop().unwrap();
                    self.stack.push(a.eq(b));
                }
                Opcode::NotEqual => {
                    let b = self.stack.pop().unwrap();
                    let a = self.stack.pop().unwrap();
                    self.stack.push(a.neq(b));
                }
                Opcode::GreaterThan => {
                    let b = self.stack.pop().unwrap();
                    let a = self.stack.pop().unwrap();
                    self.stack.push(a.gt(b));
                }
                Opcode::GreaterThanEqual => {
                    let b = self.stack.pop().unwrap();
                    let a = self.stack.pop().unwrap();
                    self.stack.push(a.gte(b));
                }
                Opcode::LessThan => {
                    let b = self.stack.pop().unwrap();
                    let a = self.stack.pop().unwrap();
                    self.stack.push(a.lt(b));
                }
                Opcode::LessThanEqual => {
                    let b = self.stack.pop().unwrap();
                    let a = self.stack.pop().unwrap();
                    self.stack.push(a.lte(b));
                }
                Opcode::Call(fn_address) => {
                    // probably need to push a new stack frame
                    let frame = StackFrame {
                        return_address: self.pc,
                        locals: vec![],
                    };

                    self.call_stack.push(frame);

                    self.pc = fn_address;
                    continue; // don't increment PC
                }
                Opcode::Return => {
                    // pop last frame off the stack
                    let frame = self.call_stack.pop().expect("Call stack underflow");
                    // and jump to its return address
                    self.pc = frame.return_address;
                }
            }

            self.pc += 1;
        }
    }

    /**
     * Used for debugging tests,
     * we pass the value to the handler instead of printing it
     *
     */
    pub fn set_output_handler<F>(&mut self, handler: F)
    where
        F: FnMut(&LeiaValue) + 'static,
    {
        self.output_handler = Some(Box::new(handler));
    }

    pub fn clear_output_handler(&mut self) -> () {
        self.output_handler = None
    }
}
