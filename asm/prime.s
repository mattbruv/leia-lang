.const 0 2           ; initial candidate
.const 1 0           ; prime counter

.main
    PUSH 0           ; candidate = 2
    STORE 0

    PUSH 1           ; primes_found = 0
    STORE 1

.loop_start
    ; set divisor = 2
    PUSH 2
    STORE 2

    ; is_prime = 1 (true)
    PUSH 1
    STORE 3

.div_check_loop
    ; if divisor * divisor > candidate, break
    LOAD 2
    LOAD 2
    MUL
    LOAD 0
    GT
    JUMPNZ check_done

    ; mod = candidate % divisor
    LOAD 0
    LOAD 2
    MOD
    STORE 5

    ; if mod == 0, not prime
    LOAD 5
    PUSH 0
    EQ
    JUMPZ skip_mark_not_prime

    ; else
    JUMP incr_divisor

.skip_mark_not_prime
    PUSH 0
    STORE 3     ; is_prime = false

.incr_divisor
    LOAD 2
    PUSH 1
    ADD
    STORE 2
    JUMP div_check_loop

.check_done
    ; if is_prime == 1
    LOAD 3
    PUSH 1
    EQ
    JUMPZ skip_count_prime

    ; primes_found += 1
    LOAD 1
    PUSH 1
    ADD
    STORE 1

    ; if primes_found == 461, print and halt
    LOAD 1
    PUSH 461
    EQ
    JUMPZ skip_print

    LOAD 0
    PRINT
    HALT

.skip_print
.skip_count_prime
    ; candidate += 1
    LOAD 0
    PUSH 1
    ADD
    STORE 0
    JUMP loop_start
