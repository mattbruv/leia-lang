.const 0 1
.const 1 200
.const 2 3


.fn_main
    PUSH_CONST 0 ; 1
    PUSH_CONST 1 ; 200
    ADD
    PUSH_CONST 2 ; 3
    DIV
    PRINT
    HALT