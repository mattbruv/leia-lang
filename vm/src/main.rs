use vm::assembler::parse_assembly;

fn main() {
    let asm = include_str!("../../asm/add.s");
    println!("{:?}", parse_assembly(asm));
}
