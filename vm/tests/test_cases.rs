use std::{cell::RefCell, rc::Rc};

use vm::{assembler::parse_assembly, vm::VM};

pub fn run_asm_test(file: &str) -> Vec<String> {
    let asm = std::fs::read_to_string(file).expect("Failed to read .s file");
    let program = parse_assembly(&asm);
    let output = Rc::new(RefCell::new(Vec::new()));
    let output_clone = Rc::clone(&output);

    let mut vm = VM::new(program);

    vm.set_output_handler(move |val| {
        output_clone.borrow_mut().push(format!("{}", val));
    });

    vm.run();
    vm.clear_output_handler();

    // return collected output
    Rc::try_unwrap(output)
        .expect("Multiple references to output exist")
        .into_inner()
}

#[cfg(test)]
mod tests {
    use crate::run_asm_test;

    #[test]
    fn test_add() {
        let val = run_asm_test("../asm/add.s");
        assert_eq!("123", val.first().unwrap());
    }

    #[test]
    fn test_fn_call() {
        let val = run_asm_test("../asm/fn_test.s");
        assert_eq!("42", val.first().unwrap());
    }

    #[test]
    fn test_euler1() {
        let val = run_asm_test("../asm/euler1.s");
        assert_eq!("233168", val.first().unwrap());
    }

    #[test]
    fn test_fib() {
        let val = run_asm_test("../asm/fib.s");
        // fib=46 = 1836311903
        assert_eq!("1836311903", val.last().unwrap());
    }

    #[test]
    fn test_prime() {
        let val = run_asm_test("../asm/prime.s");
        assert_eq!("3259", val.last().unwrap());
    }

    #[test]
    fn test_fn_factorial() {
        let val = run_asm_test("../asm/factorial.s");
        assert_eq!("362880", val.last().unwrap());
    }

    #[test]
    fn test_comparisons() {
        let val = run_asm_test("../asm/compare.s");
        let seven: Vec<bool> = val.iter().take(7).map(|x| x == "1").collect();

        assert_eq!(vec![true, false, true, false, false, true, false], seven);
    }
}
