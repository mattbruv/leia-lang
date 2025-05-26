use std::{
    env,
    fs::{self, File},
    io::Write,
    path::Path,
};

fn main() {
    let out_dir = env::var("OUT_DIR").unwrap();
    let dest_path = Path::new(&out_dir).join("generated_tests.rs");
    let mut f = File::create(&dest_path).unwrap();

    let src_dir = Path::new("../tests/src");

    for entry in fs::read_dir(src_dir).unwrap() {
        let path = entry.unwrap().path();
        if path.extension().and_then(|s| s.to_str()) != Some("leia") {
            continue;
        }

        let stem = path.file_stem().unwrap().to_str().unwrap();
        writeln!(
            f,
            r#"
#[test]
fn leia_{0}() {{
    let actual = compile_and_run("{0}");
    let expected = read_expected_output("{0}");
    assert_eq!(actual, expected, "Test failed: {0}");
}}
"#,
            stem
        )
        .unwrap();
    }
}
