.const 0 3

.main
PUSH 0 ; 3
PUSH 0 ; 3
MUL
STORE 0 ; z
LOAD 0 ; load z again
STORE 1 ; y
LOAD 1 ; load y again
STORE 2 ; x
LOAD 2 ; x
LOAD 1 ; y
ADD
LOAD 0 ; z
ADD
PRINT
HALT
