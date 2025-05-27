.const 0 0
.const 1 2
.const 2 "hi mom"

.main
    PUSH_CONST 0 ; 0
    STORE_LOCAL 0 ; x
    PUSH_CONST 1 ; 2
    LOAD_LOCAL 0 ; x
    GT
    JUMPZ L0
    PUSH_CONST 2 ; "hi mom"
    PRINT
.L0
    HALT