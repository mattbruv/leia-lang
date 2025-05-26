.const 0 1
.const 1 0
.const 2 3
.const 3 4

.main
    PUSH_CONST 0 ; 1
    JUMPZ L0
    PUSH_CONST 1 ; 0
.L0
    JUMPZ L1
    PUSH_CONST 2 ; 3
.L1
    JUMPZ L2
    PUSH_CONST 3 ; 4
.L2
    STORE_LOCAL 0 ; x
    LOAD_LOCAL 0 ; x
    PRINT
    HALT
