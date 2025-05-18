.main
    PUSH 0      ; push 42
    PUSH 1      ; push 10
    ADD         ; add
    PRINT       ; output 52
    PUSH 0
    PUSH 1
    ADD
    JP foo
    ADD
    ADD
.foo
    HALT

.const 0 42
.const 1 10
.const 1 10.5
.const 1 10.13209492409
.const 1 "foo bar baz"