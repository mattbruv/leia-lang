use crate::instruction::Program;

pub fn parse_assembly(asm: &str) -> Program {
    let mut program = Program {
        code: vec![],
        constants: vec![],
    };

    program
}
