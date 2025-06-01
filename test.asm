.const 0 1
.const 1 "hello world"
.const 2 2

.main
    PUSH_CONST 0 ; 1
    PUSH_CONST 0 ; 1
    ADD
.fn_foo
    PUSH_CONST 1 ; "hello world"
    PRINT
    RET
    PUSH_CONST 2 ; 2
    PUSH_CONST 2 ; 2
    ADD
    HALT