use std::collections::HashMap;

use crate::instruction::{ConstantIndex, ConstantValue, Opcode, Program};

pub fn parse_assembly(asm: &str) -> Program {
    let mut program = Program {
        code: parse_opcodes_with_labels(asm),
        constants: parse_constants(asm),
    };

    program
}

/// Represents an unresolved jump before label resolution
#[derive(Debug)]
enum UnresolvedOpcode {
    Resolved(Opcode),
    JpLabel(String),
}

/// Parses the input and resolves jumps
fn parse_opcodes_with_labels(asm: &str) -> Vec<Opcode> {
    let mut opcodes = Vec::new();
    let mut labels = HashMap::new();
    let mut unresolved = Vec::new();
    let mut instruction_index = 0;

    for line in asm.lines().map(str::trim).filter(|l| !l.is_empty()) {
        if line.starts_with(".const") {
            continue; // Skip constants here
        }

        if line.ends_with(':') {
            continue; // Skip labels like `main:`
        }

        if line.starts_with('.') {
            // Record the label as the *current* instruction index
            let label = line.trim_start_matches('.').to_string();
            labels.insert(label, instruction_index);
            continue;
        }

        let code = line.split(';').next().unwrap().trim(); // Remove comment
        if code.is_empty() {
            continue;
        }

        let mut parts = code.split_whitespace();
        let instr = parts.next().unwrap();

        match instr {
            "PUSH" => {
                let index_str = parts.next().expect("PUSH needs an argument");
                let index: ConstantIndex =
                    ConstantIndex(index_str.parse::<u32>().expect("Invalid constant index"));
                unresolved.push(UnresolvedOpcode::Resolved(Opcode::Push(index)));
            }
            "ADD" => unresolved.push(UnresolvedOpcode::Resolved(Opcode::Add)),
            "PRINT" => unresolved.push(UnresolvedOpcode::Resolved(Opcode::Print)),
            "HALT" => unresolved.push(UnresolvedOpcode::Resolved(Opcode::Halt)),
            "JP" => {
                let label = parts.next().expect("JP needs a label").to_string();
                unresolved.push(UnresolvedOpcode::JpLabel(label));
            }
            _ => panic!("Unknown instruction: {}", instr),
        }

        instruction_index += 1; // Count only real instructions
    }

    //println!("{:?}", labels);

    // Second pass: resolve JP labels
    for entry in unresolved {
        match entry {
            UnresolvedOpcode::Resolved(op) => opcodes.push(op),
            UnresolvedOpcode::JpLabel(label) => {
                let addr = *labels
                    .get(&label)
                    .unwrap_or_else(|| panic!("Unknown label: {label}"));
                opcodes.push(Opcode::Jump(addr));
            }
        }
    }

    opcodes
}

fn parse_constants(asm: &str) -> Vec<ConstantValue> {
    asm.lines()
        .filter(|x| x.starts_with(".const"))
        .map(|line| {
            // Split into parts and collect everything after the second token as the value
            let parts: Vec<&str> = line.split_whitespace().collect();
            let value_str = &line[line.find(parts[2]).unwrap()..];
            parse_const_value(value_str)
        })
        .collect()
}

fn parse_const_value(value: &str) -> ConstantValue {
    if let Ok(x) = value.parse::<i32>() {
        return ConstantValue::Int(x);
    }
    if let Ok(x) = value.parse::<f32>() {
        return ConstantValue::Float(x);
    }

    // Remove surrounding quotes if it's a string
    if value.starts_with('"') && value.ends_with('"') {
        return ConstantValue::Str(value[1..value.len() - 1].to_string());
    }

    panic!("Unable to parse constant: {:?}", value)
}
