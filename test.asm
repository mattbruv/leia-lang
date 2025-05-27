.const 0 2
.const 1 "hi mom"
.const 2 "otherwise!"

.main
    PUSH_CONST 0 ; 2
    STORE_LOCAL 0 ; x
    PUSH_CONST 0 ; 2
    LOAD_LOCAL 0 ; x
    GT
    JUMPZ L0
    PUSH_CONST 1 ; "hi mom"
    PRINT
    JUMP L1
.L0
    PUSH_CONST 2 ; "otherwise!"
    PRINT
.L1
    HALT