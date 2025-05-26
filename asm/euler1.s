.main
    PUSH_CONST 0       ; i = 0
    STORE_LOCAL 0

    PUSH_CONST 1       ; sum = 0
    STORE_LOCAL 1

.loop
    LOAD_LOCAL 0       ; if i >= 1000
    PUSH_CONST 2
    SUB
    JUMPZ end

    ; check i % 3
    LOAD_LOCAL 0
    PUSH_CONST 3
    MOD
    STORE_LOCAL 2

    ; check i % 5
    LOAD_LOCAL 0
    PUSH_CONST 4
    MOD
    STORE_LOCAL 3

    ; if (i % 3 == 0) || (i % 5 == 0)
    LOAD_LOCAL 2
    JUMPZ add_to_sum
    LOAD_LOCAL 3
    JUMPZ add_to_sum
    JUMP skip

.add_to_sum
    LOAD_LOCAL 1        ; sum += i
    LOAD_LOCAL 0
    ADD
    STORE_LOCAL 1

.skip
    INC 0
    JUMP loop

.end
    LOAD_LOCAL 1
    PRINT
    HALT

.const 0 0       ; initial i = 0
.const 1 0       ; initial sum = 0
.const 2 1000    ; limit = 1000
.const 3 3
.const 4 5
