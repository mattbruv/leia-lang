.main
    PUSH 0       ; i = 0
    STORE 0

    PUSH 1       ; sum = 0
    STORE 1

.loop
    LOAD 0       ; if i >= 1000
    PUSH 2
    SUB
    JUMPZ end

    ; check i % 3
    LOAD 0
    PUSH 3
    MOD
    STORE 2

    ; check i % 5
    LOAD 0
    PUSH 4
    MOD
    STORE 3

    ; if (i % 3 == 0) || (i % 5 == 0)
    LOAD 2
    JUMPZ add_to_sum
    LOAD 3
    JUMPZ add_to_sum
    JUMP skip

.add_to_sum
    LOAD 1        ; sum += i
    LOAD 0
    ADD
    STORE 1

.skip
    INC 0
    JUMP loop

.end
    LOAD 1
    PRINT
    HALT

.const 0 0       ; initial i = 0
.const 1 0       ; initial sum = 0
.const 2 1000    ; limit = 1000
.const 3 3
.const 4 5
