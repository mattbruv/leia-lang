.const 0 "hello world"
.const 1 13

.fn_foo
    PUSH_CONST 0 ; "hello world"
    PRINT
    RET
.fn_main
    PUSH_CONST 1 ; 13
    CALL fn_foo
    HALT