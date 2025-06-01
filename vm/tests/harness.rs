use std::{cell::RefCell, fs, rc::Rc};
use vm::{assembler::parse_assembly, vm::VM};

fn compile_and_run(name: &str) -> Vec<String> {
    let compiler_dll = "../Compiler/Compiler/bin/Release/net8.0/Compiler.dll";
    let src_path = format!("../tests/src/{}.leia", name);
    let out_path = format!("../tests/out/{}.asm", name);

    let output = std::process::Command::new("dotnet")
        .args([compiler_dll, &src_path])
        .output()
        .expect("Failed to run compiler");

    println!("cwd = {:?}", std::env::current_dir().unwrap());
    if !output.status.success() {
        let stderr = String::from_utf8_lossy(&output.stderr);
        let stdout = String::from_utf8_lossy(&output.stdout);
        panic!(
            "Compiler failed on '{}'\nExit status: {:?}\n--- stderr ---\n{}\n--- stdout ---\n{}",
            src_path, output.status, stderr, stdout
        );
    }

    fs::write(&out_path, &output.stdout).expect("Couldn't write asm output");

    let asm = fs::read_to_string(&out_path).expect("Couldn't read .asm");
    let program = parse_assembly(&asm);

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

fn read_expected_output(name: &str) -> Vec<String> {
    let path = format!("../tests/expected/{}.txt", name);
    fs::read_to_string(path)
        .expect("Couldn't read expected output")
        .lines()
        .map(|l| l.trim().to_string())
        .collect()
}

// 👇 This pulls in the auto-generated tests
include!(concat!(env!("OUT_DIR"), "/generated_tests.rs"));
