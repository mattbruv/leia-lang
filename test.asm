.const 0 "hello world"
.const 1 "function main"
.const 2 13


.fn_foo
    PUSH_CONST 0 ; "hello world"
    PRINT
    RET

.fn_fn_main
    PUSH_CONST 1 ; "function main"
    PRINT
    RET

.fn_main
    PUSH_CONST 2 ; 13
    CALL fn_foo
    HALT