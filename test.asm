.const 0 1
.const 1 "OK"


.fn_foo
    PUSH_CONST 0 ; 1
    PUSH_CONST 0 ; 1
    ADD
    PRINT
    PUSH_CONST 0 ; 1
    PUSH_CONST 0 ; 1
    ADD
    PRINT
    PUSH_CONST 0 ; 1
    PUSH_CONST 0 ; 1
    ADD
    PRINT
    PUSH_CONST 0 ; 1
    PUSH_CONST 0 ; 1
    ADD
    PRINT
    PUSH_CONST 0 ; 1
    PUSH_CONST 0 ; 1
    ADD
    PRINT
    RET

.fn_main
    PUSH_CONST 1 ; "OK"
    PRINT
    HALT