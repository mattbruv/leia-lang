use crate::instruction::Opcode;

#[derive(Debug)]
pub struct VM {
    pc: usize,
    program: Vec<u8>,
}

impl VM {
    pub fn new() -> VM {
        VM {
            pc: 0,
            program: vec![],
        }
    }

    pub fn run(&mut self) -> () {
        loop {
            if self.pc >= self.program.len() {
                break;
            }

            match self.decode_opcode() {
                Opcode::Halt => {
                    println!("Halt encountered");
                    return;
                }
                _ => {
                    println!("Unhandled opcode found! Terminating!");
                    return;
                }
            }
        }
    }

    fn decode_opcode(&mut self) -> Opcode {
        let opcode = Opcode::from(self.program[self.pc]);
        self.pc += 1;
        return opcode;
    }
}

#[cfg(test)]
mod tests {
    use super::*;
    #[test]
    fn test_opcode_halt() {
        let mut test_vm = VM::new();
        let test_bytes = vec![0, 0, 0, 0];
        test_vm.program = test_bytes;
        test_vm.run();
        assert_eq!(test_vm.pc, 1);
    }

    #[test]
    fn test_opcode_illegal() {
        let mut test_vm = VM::new();
        let test_bytes = vec![200, 0, 0, 0];
        test_vm.program = test_bytes;
        test_vm.run();
        assert_eq!(test_vm.pc, 1);
    }
}
