.const 0 21     ; The number to double
.const 1 2      ; Constant multiplier

.main
    PUSH 0      ; Push 21
    CALL double
    HALT

.double
    STORE 0     ; Store input into local[0]
    LOAD 0      ; Load local[0]
    PUSH 1      ; Push 2
    MUL         ; Multiply local[0] * 2
    PRINT       ; Print result
    RET
