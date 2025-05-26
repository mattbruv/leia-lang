.const 0 1
.const 1 2
.const 2 3

.main
    PUSH_CONST 0 ; 1
    PUSH_CONST 1 ; 2
    LT
    PRINT
    PUSH_CONST 1 ; 2
    PUSH_CONST 1 ; 2
    LT
    PRINT
    PUSH_CONST 1 ; 2
    PUSH_CONST 1 ; 2
    LTE
    PRINT
    PUSH_CONST 1 ; 2
    PUSH_CONST 2 ; 3
    GT
    PRINT
    PUSH_CONST 1 ; 2
    PUSH_CONST 2 ; 3
    GTE
    PRINT
    PUSH_CONST 2 ; 3
    PUSH_CONST 2 ; 3
    GTE
    PRINT
    PUSH_CONST 2 ; 3
    PUSH_CONST 2 ; 3
    GT
    PRINT
    HALT