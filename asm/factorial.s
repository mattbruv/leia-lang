.const 0 9      ; number to calculate factorial of
.const 1 1      ; temp/local 0
.const 2 0      ; temp/local 1 (for recursion)

.main
    PUSH 0         ; Push constant 5
    CALL factorial
    PRINT
    HALT

.factorial
    STORE 0       ; store argument n in local 0

    LOAD 0        ; load n
    PUSH 1
    SUB
    JUMPZ base_case  ; if n-1 == 0 -> base case

    LOAD 0
    PUSH 1
    SUB
    CALL factorial    ; recurse factorial(n-1)

    LOAD 0
    MUL               ; multiply n * factorial(n-1)
    RET

.base_case
    PUSH 1         ; factorial(1) = 1
    RET
