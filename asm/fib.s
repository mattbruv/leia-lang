; prints the first 46 fibonacci numbers

.const 0 47         ; n = 7
.const 1 0         ; a = 0
.const 2 1         ; b = 1
.const 3 2         ; counter starts at 2

.main
  PUSH 0           ; load n
  STORE 0

  PUSH 1           ; a = 0
  STORE 1

  PUSH 2           ; b = 1
  STORE 2

  PUSH 3           ; counter = 2
  STORE 3

.loop_start
  LOAD 3           ; counter
  LOAD 0           ; n
  SUB              ; counter - n
  JUMPZ end   ; if counter == n, end loop

  LOAD 1           ; a
  LOAD 2           ; b
  ADD              ; a + b
  STORE 4          ; temp = a + b

  LOAD 2
  STORE 1          ; a = b
  
  LOAD 2
  PRINT

  LOAD 4
  STORE 2          ; b = temp

  LOAD 3
  PUSH 2
  ADD
  STORE 3          ; counter += 1

  JUMP loop_start

.end
  LOAD 2           ; result is in b
  PRINT
  HALT
