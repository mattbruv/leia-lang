.const 0 0
.const 1 2
.const 2 5
.const 3 1
.const 4 3

.main
    PUSH_CONST 0 ; 0
    JUMPNZ L1
    POP
    PUSH_CONST 1 ; 2
    JUMPZ L0
    POP
    PUSH_CONST 2 ; 5
.L0
.L1
    STORE_LOCAL 0 ; x
    PUSH_CONST 3 ; 1
    JUMPZ L2
    POP
    PUSH_CONST 1 ; 2
.L2
    JUMPNZ L3
    POP
    PUSH_CONST 4 ; 3
.L3
    STORE_LOCAL 1 ; y
    LOAD_LOCAL 0 ; x
    PRINT
    LOAD_LOCAL 1 ; y
    PRINT
    HALT

