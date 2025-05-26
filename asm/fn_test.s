.const 0 21     ; The number to double
.const 1 2      ; Constant multiplier

.main
    PUSH_CONST 0      ; Push 21
    CALL double
    HALT

.double
    STORE_LOCAL 0     ; Store input into local[0]
    LOAD_LOCAL 0      ; Load local[0]
    PUSH_CONST 1      ; Push 2
    MUL         ; Multiply local[0] * 2
    PRINT       ; Print result
    RET
