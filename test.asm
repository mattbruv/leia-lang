.const 0 1
.const 1 2
.const 2 3
.const 3 4

.main
    PUSH_CONST 0 ; 1
    PUSH_CONST 1 ; 2
    ADD
    PUSH_CONST 2 ; 3
    ADD
    PUSH_CONST 3 ; 4
    ADD
    PRINT
    HALT