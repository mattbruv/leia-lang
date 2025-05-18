use vm::assembler::parse_assembly;
use vm::vm::VM;

fn main() {
    let asm_text = include_str!("../../asm/add.s");
    let asm = parse_assembly(asm_text);
    let mut vm = VM::new(asm);
    vm.run();
}
