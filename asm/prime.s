.const 0 2       ; initial candidate
.const 1 0       ; initial primes found
.const 2 461     ; target prime count
.const 3 1       ; constant 1
.const 4 0       ; constant 0

.main
    PUSH 0       ; candidate = 2
    STORE 0

    PUSH 1       ; primes_found = 0
    STORE 1

.loop_start
    PUSH 0       ; divisor = 2
    STORE 2

    PUSH 3       ; is_prime = 1
    STORE 3

.div_check
    LOAD 2       ; divisor * divisor > candidate
    LOAD 2
    MUL
    LOAD 0
    GT
    JUMPNZ done_checking

    LOAD 0       ; candidate % divisor
    LOAD 2
    MOD
    STORE 4

    LOAD 4       ; if mod_result == 0
    PUSH 4
    EQ
    JUMPZ skip_mark_not_prime

    PUSH 4       ; is_prime = 0
    STORE 3

.skip_mark_not_prime
    LOAD 2       ; divisor += 1
    PUSH 3
    ADD
    STORE 2
    JUMP div_check

.done_checking
    LOAD 3       ; if is_prime != 1, skip increment
    PUSH 3
    EQ
    JUMPZ skip_increment

    LOAD 1       ; primes_found += 1
    PUSH 3
    ADD
    STORE 1

    LOAD 1       ; if primes_found == 461
    PUSH 2
    EQ
    JUMPZ skip_print

    LOAD 0       ; print candidate
    PRINT
    HALT

.skip_print
.skip_increment
    LOAD 0       ; candidate += 1
    PUSH 3
    ADD
    STORE 0
    JUMP loop_start
