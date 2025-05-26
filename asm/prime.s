.const 0 2       ; initial candidate
.const 1 0       ; initial primes found
.const 2 461     ; target prime count
.const 3 1       ; constant 1
.const 4 0       ; constant 0

.main
    PUSH_CONST 0       ; candidate = 2
    STORE_LOCAL 0

    PUSH_CONST 1       ; primes_found = 0
    STORE_LOCAL 1

.loop_start
    PUSH_CONST 0       ; divisor = 2
    STORE_LOCAL 2

    PUSH_CONST 3       ; is_prime = 1
    STORE_LOCAL 3

.div_check
    LOAD_LOCAL 2       ; divisor * divisor > candidate
    LOAD_LOCAL 2
    MUL
    LOAD_LOCAL 0
    GT
    JUMPNZ done_checking

    LOAD_LOCAL 0       ; candidate % divisor
    LOAD_LOCAL 2
    MOD
    STORE_LOCAL 4

    LOAD_LOCAL 4       ; if mod_result == 0
    PUSH_CONST 4
    EQ
    JUMPZ skip_mark_not_prime

    PUSH_CONST 4       ; is_prime = 0
    STORE_LOCAL 3

.skip_mark_not_prime
    INC 2       ; divisor += 1
    JUMP div_check

.done_checking
    LOAD_LOCAL 3       ; if is_prime != 1, skip increment
    PUSH_CONST 3
    EQ
    JUMPZ skip_increment

    INC 1 ; primes_found += 1

    LOAD_LOCAL 1       ; if primes_found == 461
    PUSH_CONST 2
    EQ
    JUMPZ skip_print

    LOAD_LOCAL 0       ; print candidate
    PRINT
    HALT

.skip_print
.skip_increment
    INC 0 ; candidate += 1
    JUMP loop_start
