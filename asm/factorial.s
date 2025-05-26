.const 0 9      ; number to calculate factorial of
.const 1 1      ; temp/local 0
.const 2 0      ; temp/local 1 (for recursion)

.main
    PUSH_CONST 0         ; Push constant 5
    CALL factorial
    PRINT
    HALT

.factorial
    STORE_LOCAL 0       ; store argument n in local 0

    LOAD_LOCAL 0        ; load n
    PUSH_CONST 1
    SUB
    JUMPZ base_case  ; if n-1 == 0 -> base case
    POP

    LOAD_LOCAL 0
    PUSH_CONST 1
    SUB
    CALL factorial    ; recurse factorial(n-1)

    LOAD_LOCAL 0
    MUL               ; multiply n * factorial(n-1)
    RET

.base_case
    PUSH_CONST 1         ; factorial(1) = 1
    RET
