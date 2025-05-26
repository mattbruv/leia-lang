; prints the first 46 fibonacci numbers

.const 0 47         ; n = 7
.const 1 0         ; a = 0
.const 2 1         ; b = 1
.const 3 2         ; counter starts at 2

.main
  PUSH_CONST 0           ; load n
  STORE_LOCAL 0

  PUSH_CONST 1           ; a = 0
  STORE_LOCAL 1

  PUSH_CONST 2           ; b = 1
  STORE_LOCAL 2

  PUSH_CONST 3           ; counter = 2
  STORE_LOCAL 3

.loop_start
  LOAD_LOCAL 3           ; counter
  LOAD_LOCAL 0           ; n
  SUB              ; counter - n
  JUMPZ end   ; if counter == n, end loop
  POP

  LOAD_LOCAL 1           ; a
  LOAD_LOCAL 2           ; b
  ADD              ; a + b
  STORE_LOCAL 4          ; temp = a + b

  LOAD_LOCAL 2
  STORE_LOCAL 1          ; a = b
  
  LOAD_LOCAL 2
  PRINT

  LOAD_LOCAL 4
  STORE_LOCAL 2          ; b = temp

  LOAD_LOCAL 3
  PUSH_CONST 2
  ADD
  STORE_LOCAL 3          ; counter += 1

  JUMP loop_start

.end
  LOAD_LOCAL 2           ; result is in b
  PRINT
  HALT
