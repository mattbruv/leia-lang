.const 0 1
.const 1 2
.const 2 3
.const 3 4
.const 4 5

.main
    PUSH_CONST 0 ; 1
    JUMPZ L0
    POP
    PUSH_CONST 1 ; 2
.L0
    JUMPZ L1
    POP
    PUSH_CONST 2 ; 3
.L1
    JUMPZ L2
    POP
    PUSH_CONST 3 ; 4
.L2
    JUMPZ L3
    POP
    PUSH_CONST 4 ; 5
.L3
    STORE_LOCAL 0 ; x
    LOAD_LOCAL 0 ; x
    PRINT
    HALT
