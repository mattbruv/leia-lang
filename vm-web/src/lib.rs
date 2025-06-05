mod utils;

use std::{cell::RefCell, rc::Rc};

use vm::{assembler::parse_assembly, vm::VM};
use wasm_bindgen::prelude::*;

// #[wasm_bindgen]
// extern "C" {
//     fn alert(s: &str);
// }

#[wasm_bindgen]
pub fn run_asm(asm_text: &str) -> Vec<String> {
    utils::set_panic_hook();
    let program = parse_assembly(&asm_text);

    let output = Rc::new(RefCell::new(Vec::new()));
    let output_clone = Rc::clone(&output);

    let mut vm = VM::new(program);
    vm.set_output_handler(move |val| {
        output_clone.borrow_mut().push(format!("{}", val));
    });
    vm.run();
    vm.clear_output_handler();
    Rc::try_unwrap(output).unwrap().into_inner()
}
