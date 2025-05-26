use std::collections::HashMap;

use crate::instruction::{ConstantIndex, ConstantValue, Opcode, Program};

pub fn parse_assembly(asm: &str) -> Program {
    Program {
        code: parse_opcodes_with_labels(asm),
        constants: parse_constants(asm),
    }
}

/// Represents an unresolved jump before label resolution
#[derive(Debug)]
enum UnresolvedOpcode {
    Resolved(Opcode),
    CallLabel(String),
    JumpLabel(String),
    JumpZeroLabel(String),
    JumpNotZeroLabel(String),
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
            //println!("{:?} {:?}", line, instruction_index);
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
            "PUSH_CONST" => {
                let index_str = parts.next().expect("PUSH_CONST needs an argument");
                let index: ConstantIndex =
                    ConstantIndex(index_str.parse::<u32>().expect("Invalid constant index"));
                unresolved.push(UnresolvedOpcode::Resolved(Opcode::Push(index)));
            }
            "STORE_LOCAL" => {
                let index_str = parts.next().expect("STORE_LOCAL needs an argument");
                let index: usize = index_str.parse::<usize>().expect("Invalid constant index");
                unresolved.push(UnresolvedOpcode::Resolved(Opcode::StoreLocal(index)));
            }
            "LOAD_LOCAL" => {
                let index_str = parts.next().expect("LOAD_LOCAL needs an argument");
                let index: usize = index_str.parse::<usize>().expect("Invalid constant index");
                unresolved.push(UnresolvedOpcode::Resolved(Opcode::LoadLocal(index)));
            }
            "ADD" => unresolved.push(UnresolvedOpcode::Resolved(Opcode::Add)),
            "SUB" => unresolved.push(UnresolvedOpcode::Resolved(Opcode::Subtract)),
            "MUL" => unresolved.push(UnresolvedOpcode::Resolved(Opcode::Multiply)),
            "DIV" => unresolved.push(UnresolvedOpcode::Resolved(Opcode::Divide)),
            "MOD" => unresolved.push(UnresolvedOpcode::Resolved(Opcode::Modulo)),
            "PRINT" => unresolved.push(UnresolvedOpcode::Resolved(Opcode::Print)),
            "EQ" => unresolved.push(UnresolvedOpcode::Resolved(Opcode::Equals)),
            "INC" => {
                let index_str = parts.next().expect("INC needs an argument");
                let index: usize = index_str.parse::<usize>().expect("Invalid constant index");
                unresolved.push(UnresolvedOpcode::Resolved(Opcode::Increment(index)));
            }
            "GT" => unresolved.push(UnresolvedOpcode::Resolved(Opcode::GreaterThan)),
            "GTE" => unresolved.push(UnresolvedOpcode::Resolved(Opcode::GreaterThanEqual)),
            "LT" => unresolved.push(UnresolvedOpcode::Resolved(Opcode::LessThan)),
            "LTE" => unresolved.push(UnresolvedOpcode::Resolved(Opcode::LessThanEqual)),
            "HALT" => unresolved.push(UnresolvedOpcode::Resolved(Opcode::Halt)),
            "JUMP" => {
                let label = parts.next().expect("JUMP needs a label").to_string();
                unresolved.push(UnresolvedOpcode::JumpLabel(label));
            }
            "CALL" => {
                let label = parts.next().expect("CALL needs a label").to_string();
                unresolved.push(UnresolvedOpcode::CallLabel(label));
            }
            "RET" => {
                unresolved.push(UnresolvedOpcode::Resolved(Opcode::Return));
            }
            "JUMPZ" => {
                let label = parts.next().expect("JUMPZ needs a label").to_string();
                unresolved.push(UnresolvedOpcode::JumpZeroLabel(label));
            }
            "JUMPNZ" => {
                let label = parts.next().expect("JUMPZ needs a label").to_string();
                unresolved.push(UnresolvedOpcode::JumpNotZeroLabel(label));
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
            UnresolvedOpcode::JumpLabel(label) => {
                let addr = *labels
                    .get(&label)
                    .unwrap_or_else(|| panic!("Unknown label: {label}"));
                opcodes.push(Opcode::Jump(addr));
            }
            UnresolvedOpcode::JumpZeroLabel(label) => {
                let addr = *labels
                    .get(&label)
                    .unwrap_or_else(|| panic!("Unknown label: {label}"));
                opcodes.push(Opcode::JumpIfZero(addr));
            }
            UnresolvedOpcode::JumpNotZeroLabel(label) => {
                let addr = *labels
                    .get(&label)
                    .unwrap_or_else(|| panic!("Unknown label: {label}"));
                opcodes.push(Opcode::JumpIfNotZero(addr));
            }
            UnresolvedOpcode::CallLabel(label) => {
                let addr = *labels
                    .get(&label)
                    .unwrap_or_else(|| panic!("Unknown label: {label}"));
                opcodes.push(Opcode::Call(addr));
            }
        }
    }

    opcodes
}

fn parse_constants(asm: &str) -> Vec<ConstantValue> {
    asm.lines()
        .filter(|x| x.trim_start().starts_with(".const"))
        .map(|x| {
            let no_comment = x.split(';').next().unwrap().trim();
            no_comment
        })
        .map(|line| {
            let parts: Vec<&str> = line.split_whitespace().collect();
            if parts.len() < 3 {
                panic!("Malformed .const line: {}", line);
            }
            let value_str = parts[2..].join(" ");
            parse_const_value(&value_str)
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
