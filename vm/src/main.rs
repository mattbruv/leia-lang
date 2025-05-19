use std::time::Instant;

use vm::assembler::parse_assembly;
use vm::vm::VM;

fn main() {
    let asm_text = include_str!("../../asm/prime.s");
    let asm = parse_assembly(asm_text);
    let mut vm = VM::new(asm);
    let start = Instant::now();
    vm.run();
    println!("elapsed: {:?}", start.elapsed());
}
